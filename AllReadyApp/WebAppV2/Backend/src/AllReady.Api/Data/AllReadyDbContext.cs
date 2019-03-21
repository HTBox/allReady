using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace AllReady.Api.Data
{
    public class AllReadyDbContext : DbContext
    {
        public AllReadyDbContext(DbContextOptions<AllReadyDbContext> options) : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var localDateConverter = new ValueConverter<LocalDate, DateTime>(v =>
                v.ToDateTimeUnspecified(), v => LocalDate.FromDateTime(v));

            var timeZoneConverter = new ValueConverter<DateTimeZone, string>(v =>
                v.Id, v => DateTimeZoneProviders.Tzdb[v]);

            modelBuilder.Entity<Campaign>()
                .Property(e => e.StartDateTime)
                .HasConversion(localDateConverter);

            modelBuilder.Entity<Campaign>()
                .Property(e => e.EndDateTime)
                .HasConversion(localDateConverter);

            modelBuilder.Entity<Campaign>()
                .Property(e => e.TimeZone)
                .HasConversion(timeZoneConverter);
        }
    }
}
