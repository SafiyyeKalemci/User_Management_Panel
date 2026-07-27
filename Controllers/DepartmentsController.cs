using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var departments = _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .ToList();

            ViewBag.Managers = _context.Users.Where(u => u.IsManager).ToList();

            return View(departments);
        }

        [HttpPost]
        public IActionResult Edit(Department department)
        {
            var existing = _context.Departments.Find(department.Id);
            if (existing != null)
            {
                existing.Name = department.Name;
                existing.ManagerId = department.ManagerId;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null)
            {
                var hasUsers = _context.Users.Any(u => u.DepartmentId == id);
                if (hasUsers)
                {
                    TempData["ErrorMessage"] = "Bu departmana bağlı kullanıcılar var, önce onları başka bir departmana taşıyın.";
                    return RedirectToAction("Index");
                }
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}