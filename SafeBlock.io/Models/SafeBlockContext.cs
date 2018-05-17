using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SafeBlock.Io.Models;

namespace SafeBlock.Io.Models
{
    public sealed partial class SafeBlockContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        public SafeBlockContext() : base()
        {
        }

        public SafeBlockContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO : VIRER CA
            #region old
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Database=SafeBlock;Username=safeblock;Password=rafale");
            }
            #endregion
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.ForNpgsqlHasComment("Contain all users informations.");

                entity.HasIndex(e => e.Mail)
                    .HasName("MailPrevent")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Mail).IsRequired();

                entity.Property(e => e.Token).IsRequired();
                
                entity.Property(e => e.Role).IsRequired();

                entity.Property(e => e.RegisterDate).HasColumnType("date");

                entity.Property(e => e.RegisterIp).HasColumnName("RegisterIP");
            });
        }
    }
}
