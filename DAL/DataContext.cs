using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<MetaInfo> MetaInfoSet { get; set; }
        public DbSet<ChunkInfo> ChunkInfoSet { get; set; }
        public DbSet<LocationInfo> LocationInfoSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetaInfo>().HasIndex(f => f.Name).IsUnique();
            modelBuilder.Entity<LocationInfo>().HasIndex(f => f.Path).IsUnique();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
