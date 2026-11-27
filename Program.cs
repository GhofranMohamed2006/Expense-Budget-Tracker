using Pennywise.BLL.Abstractions;
using Pennywise.BLL.Services;
using Pennywise.DAL.Abstractions;
using Pennywise.DAL.Integration;
using Reports_Integration.PL.Security;

namespace Reports_Integration.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // Reports Module - Dependency Injection
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<
                ICurrentUserContext,
                CurrentUserContext>();

            builder.Services.AddScoped<
                IReportsService,
                ReportsService>();

            builder.Services.AddScoped<
                ITransactionReportRepository,
                UnconfiguredTransactionReportRepository>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapControllers();

            app.Run();
        }
    }
}
