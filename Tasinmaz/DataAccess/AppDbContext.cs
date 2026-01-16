using Microsoft.EntityFrameworkCore;
using Tasinmaz.Entities.Concrete;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Tasinmaz.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Il> Iller { get; set; } = null!;
        public DbSet<Ilce> Ilceler { get; set; } = null!;
        public DbSet<Mahalle> Mahalle { get; set; } = null!;
        public DbSet<Tasinmaz.Entities.Concrete.Tasinmaz> Tasinmaz { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Log> Log { get; set; } = null!;
        public DbSet<AnalysisGeometry> AnalysisGeometry { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tasinmaz.Entities.Concrete.Tasinmaz>()
    .Property(t => t.LocationGeometry)
    .HasColumnType("geometry");


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnalysisGeometry>(entity =>
            {
                entity.ToTable("AnalysisGeometry");

                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(1);   

                entity.Property(e => e.Geometry)
                      .HasColumnType("geometry");  

                entity.Property(e => e.AreaM2)
                      .HasColumnType("double precision");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("NOW()");
            });


        }

    }
}

