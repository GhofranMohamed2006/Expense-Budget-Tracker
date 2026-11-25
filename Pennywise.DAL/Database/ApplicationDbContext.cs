using Microsoft.EntityFrameworkCore;
using Pennywise.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pennywise.DAL.Database
{
    internal class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=GhofGhof;Database=Pennywise;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
        }
        public DbSet<Budget> Budgets { get; set; }
    }
}
