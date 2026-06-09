using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  // ← ДОБАВИТЬ
using Microsoft.AspNetCore.Identity;                      // ← ДОБАВИТЬ
using MyCRM.Domain.Entities;

namespace MyCRM.Infrastructure.Persistence
{
    // ===== ИЗМЕНИТЬ НАСЛЕДОВАНИЕ =====
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<WorkTask> WorkTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== ВАЖНО: ВЫЗВАТЬ base ПЕРВЫМ! =====
            base.OnModelCreating(modelBuilder);

            // ===== CLIENTS =====
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Clients");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).UseIdentityColumn();
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Surname).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Age).IsRequired();
                entity.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property("_viewCount").HasColumnName("ViewCount").HasDefaultValue(0);
                entity.Property(c => c.AvatarPath).HasMaxLength(500).IsRequired(false);

                // ===== ДОБАВИТЬ СВЯЗЬ С ПОЛЬЗОВАТЕЛЕМ =====
                entity.HasOne(c => c.CreatedBy)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Contacts)
                    .WithOne(c => c.Client)
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Deals)
                    .WithOne(d => d.Client)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== CONTACTS =====
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).UseIdentityColumn();
                entity.Property(c => c.Phone).HasMaxLength(20);
                entity.Property(c => c.Email).HasMaxLength(200);

                entity.HasIndex(c => c.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Contacts_Email");

                entity.HasOne(c => c.Client)
                    .WithMany(c => c.Contacts)
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== DEALS =====
            modelBuilder.Entity<Deal>(entity =>
            {
                entity.ToTable("Deals");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).UseIdentityColumn();
                entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");
                entity.Property(d => d.Count).HasColumnType("decimal(18,2)");
                entity.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(d => d.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(d => d.Deadline).IsRequired();
                entity.Ignore(d => d.Total);
                entity.Property(d => d.CreatedByUserId).HasColumnType("text");

                entity.HasIndex(d => d.Status).HasDatabaseName("IX_Deals_Status");
                entity.HasIndex(d => d.ClientId).HasDatabaseName("IX_Deals_ClientId");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Client)
                    .WithMany(c => c.Deals)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(d => d.Tasks)
                    .WithOne(t => t.Deal)
                    .HasForeignKey(t => t.DealId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== WORK TASKS =====
            modelBuilder.Entity<WorkTask>(entity =>
            {
                entity.ToTable("WorkTasks");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).UseIdentityColumn();
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.DueDate).IsRequired();
                entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

                entity.HasIndex(t => t.DueDate).HasDatabaseName("IX_WorkTasks_Deadline");

                entity.HasOne(t => t.Deal)
                    .WithMany(d => d.Tasks)
                    .HasForeignKey(t => t.DealId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}