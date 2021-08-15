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

        public DbSet<FileInfo> FileInfoSet { get; set; }
        public DbSet<ChunkInfo> ChunkInfoSet { get; set; }

        //todo - dont hardcode
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=localhost\\SQLEXPRESS;database=FileChunkerDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
