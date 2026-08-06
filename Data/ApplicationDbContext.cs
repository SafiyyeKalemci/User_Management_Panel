using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models;

namespace UserManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Identity tablolarının (AspNetUsers, AspNetRoles vb.) kurulması için ŞART

            // Kişinin kendi amiri (self-referencing FK)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcının bağlı olduğu departman
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Departmanın yöneticisi (bir User)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1) Departman önce ManagerId=null ile ekleniyor (cycle oluşmasın diye)
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Sales", ManagerId = null }
            );

            // 2) Kullanıcılar departmana bağlayarak ekleniyor
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Nancy", Surname = "Davolio", Email = "nancy.davolio@company.com", DepartmentId = 1, IsActive = true, ManagerId = 2 },
                new User { Id = 2, Name = "Andrew", Surname = "Fuller", Email = "andrew.fuller@company.com", DepartmentId = 1, IsActive = true, ManagerId = null },
                new User { Id = 3, Name = "Janet", Surname = "Leverling", Email = "janet.leverling@company.com", DepartmentId = 1, IsActive = true, ManagerId = 2 },
                new User { Id = 4, Name = "Margaret", Surname = "Peacock", Email = "margaret.peacock@company.com", DepartmentId = 1, IsActive = true, ManagerId = 2 },
                new User { Id = 5, Name = "Steven", Surname = "Buchanan", Email = "steven.buchanan@company.com", DepartmentId = 1, IsActive = true, ManagerId = 2 },
                new User { Id = 6, Name = "Michael", Surname = "Suyama", Email = "michael.suyama@company.com", DepartmentId = 1, IsActive = true, ManagerId = 5 },
                new User { Id = 7, Name = "Robert", Surname = "King", Email = "robert.king@company.com", DepartmentId = 1, IsActive = false, ManagerId = 5 },
                new User { Id = 8, Name = "Laura", Surname = "Callahan", Email = "laura.callahan@company.com", DepartmentId = 1, IsActive = true, ManagerId = 2 },
                new User { Id = 9, Name = "Anne", Surname = "Dodsworth", Email = "anne.dodsworth@company.com", DepartmentId = 1, IsActive = false, ManagerId = 5 }
            );
        }
    }
}