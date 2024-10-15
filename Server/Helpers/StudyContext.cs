using Microsoft.EntityFrameworkCore;
using System;
using Projekt.Shared.Entities;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Projekt.Server.Helpers
{
    public class StudyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<SentItem> SentItem { get; set; }
        public DbSet<SentQuestion> SentQuestion { get; set; }

        public StudyContext(DbContextOptions<StudyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role role = new() { RoleId = 1, Name = "Admin" };
            var admin = new { UserId = 1, Login = "admin", Password = BCryptNet.HashPassword("admin"), CreatedDate = DateTime.UtcNow, RoleId = 1 };
            User user = new() { UserId = 2, Login = "student", Password = BCryptNet.HashPassword("student"), CreatedDate = DateTime.UtcNow };

            modelBuilder.Entity<Role>().HasData(role);
            modelBuilder.Entity<User>().HasData(admin, user);
        }
    }
}
