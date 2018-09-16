using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SafeBlock.io.Models;

namespace SafeBlock.io.Models
{
    public sealed partial class SafeBlockContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Blog { get; set; }
        
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
            modelBuilder.Entity<User>(entity =>
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

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ForNpgsqlHasComment("Contain all blog articles.");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WriteDate).HasColumnType("date");

                entity.Property(e => e.Author).IsRequired();
                
                entity.Property(e => e.ViewsNumber).IsRequired();

                /*entity.HasIndex(e => e.Mail)
                    .HasName("MailPrevent")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Mail).IsRequired();

                entity.Property(e => e.Token).IsRequired();
                
                entity.Property(e => e.Role).IsRequired();

                entity.Property(e => e.RegisterDate).HasColumnType("date");

                entity.Property(e => e.RegisterIp).HasColumnName("RegisterIP");*/
            });
        }
    }
}
