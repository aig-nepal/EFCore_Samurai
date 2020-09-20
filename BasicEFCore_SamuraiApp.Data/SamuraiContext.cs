using BasicEFCore_SamuraiApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicEFCore_SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<Battle> Battles { get; set; }




        public static readonly ILoggerFactory ConsoleloggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder
                .AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information)
                .AddConsole();
            });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder); 
            optionsBuilder.UseLoggerFactory(loggerFactory: ConsoleloggerFactory).EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(
                connectionString: "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiDB",
                option => option.MaxBatchSize(100)
                );
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many to Many             
            modelBuilder.Entity<SamuraiBattle>().HasKey(x => new { x.SamuraiId, x.BattleId });
            modelBuilder.Entity<Horse>().ToTable("Horses");
        }


    }
}
