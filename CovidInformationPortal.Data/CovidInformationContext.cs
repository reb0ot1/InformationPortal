using Microsoft.EntityFrameworkCore;
using System;

namespace CovidInformationPortal.Data
{
    public class CovidInformationContext : DbContext
    {
        public CovidInformationContext(DbContextOptions<CovidInformationContext> options) : base(options)
        {
        }

        public DbSet<ReadTemplate> ReadTemplate { get; set; }
        public DbSet<DayInformation> DayInformation { get; set; }
        public DbSet<LostBattle> LostBattle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
