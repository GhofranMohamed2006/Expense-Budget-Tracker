# Pennywise — Backend (N-Tier)

Common <- DAL <- BLL <- Presentation (Presentation -> BLL + Common)

## تشغيل أول مرة
```bash
dotnet restore
# سر الـ JWT (مش بنكتبه في git)
cd Pennywise.Presentation
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "put-a-random-string-of-at-least-32-chars-here"
cd ..
# Migration (مسؤول الـ DB بس هو اللي يعملها)
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p Pennywise.DAL -s Pennywise.Presentation -o Migrations
dotnet ef database update -p Pennywise.DAL -s Pennywise.Presentation
dotnet run --project Pennywise.Presentation
```
Swagger: https://localhost:<port>/swagger

## Auth endpoints
| Method | Route | Auth |
|---|---|---|
| POST | /api/auth/register | no |
| POST | /api/auth/login | no |
| GET | /api/auth/me | Bearer |

## لباقي الفريق
- اجيبوا الـ UserId من `User.GetUserId()` (Presentation/Extensions) — ماتستقبلوهوش من العميل.
- ارموا الأخطاء بـ `NotFoundException / ForbiddenException / ConflictException / ValidationFailedException` (Common) والـ middleware بيحوّلها لـ ProblemDetails.
- سجلوا الـ Services بتاعتكم في `Pennywise.BLL/DependencyInjection.cs`.
- أي Entity جديدة: ضيفوا `IEntityTypeConfiguration` في DAL/Configurations (بتتحمل تلقائي) و DbSet في ApplicationDbContext.
