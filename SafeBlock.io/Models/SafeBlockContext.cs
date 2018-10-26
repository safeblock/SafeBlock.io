using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeBlock.io.Models.DatabaseModels;

namespace SafeBlock.io.Models
{
    public class SafeBlockContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<Article> Blog { get; set; }
        public DbSet<SupportArticle> Support { get; set; }
        
        public SafeBlockContext() : base()
        {
        }

        public SafeBlockContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<ApplicationUser>().Property(p => p.Id).UseNpgsqlIdentityColumn();
            /*modelBuilder.Entity<User>(entity =>
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
            });

            modelBuilder.Entity<SupportArticle>(entity =>
            {
                entity.ForNpgsqlHasComment("Contains all support articles.");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WriteDate).HasColumnType("date");
            });*/
        }
    }
}
