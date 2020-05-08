using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using WebApi.Data.Models;

namespace WebApi.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<AvatarInDb> Avatars { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }
        
        public DbSet<ItemRelation> ItemsRelations { get; set; }

        public DbSet<UserFoundModel> FoundUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.User)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.AssignedUserId);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.ItemType)
                .WithMany(it => it.Items)
                .HasForeignKey(i => i.TypeId);

            //modelBuilder.Entity<User>()
            //	.HasKey(u => new { u.Id });

            modelBuilder.Entity<Project>()
                .HasKey(p => new { p.Id });

            modelBuilder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.UserId, pu.ProjectId });

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.UserRole)
                .WithMany(aur => aur.ProjectUserRecords)
                .HasForeignKey(pu => pu.UserRoleId);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectsUsers)
                .HasForeignKey(pu => pu.ProjectId);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ProjectsUsers)
                .HasForeignKey(pu => pu.UserId);

            modelBuilder.Entity<AvatarInDb>()
                .HasKey(a => new { a.UserId });

            modelBuilder.Entity<AvatarInDb>()
                .HasOne(a => a.User)
                .WithOne(u => u.Avatar)
                .HasForeignKey<AvatarInDb>(a => a.UserId);

            modelBuilder.Entity<AppUserRole>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<ItemRelation>().HasKey(bc => new { bc.FirstItemId, bc.SecondItemId });
            modelBuilder.Entity<ItemRelation>().HasOne(i => i.FirstItem).WithMany(i => i.ItemsRelations).HasForeignKey(i => i.FirstItemId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ItemRelation>().HasOne(i => i.SecondItem).WithMany(i => i.ItemsRelationsOf).HasForeignKey(i => i.SecondItemId);
        }
    }
}