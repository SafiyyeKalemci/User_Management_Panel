using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tüm departmanları, bağlı oldukları yönetici ve çalışan bilgileriyle
        /// birlikte listeler. Departman düzenleme formunda kullanılacak yönetici
        /// adaylarını (IsManager = true olan kullanıcılar) ayrıca hazırlar.
        /// </summary>
        public IActionResult Index()
        {
            var departments = _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .ToList();

            ViewBag.Managers = _context.Users.Where(u => u.IsManager).ToList();

            return View(departments);
        }

        /// <summary>
        /// Var olan bir departmanın adını ve/veya atanmış yöneticisini günceller.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(Department department)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Departman adı boş bırakılamaz.";
                return RedirectToAction("Index");
            }

            var existing = _context.Departments.Find(department.Id);
            if (existing != null)
            {
                existing.Name = department.Name;
                existing.ManagerId = department.ManagerId;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /*
        /// <summary>
        /// Bir departmanı, yalnızca o departmana bağlı hiçbir kullanıcı yoksa
        /// kalıcı olarak siler. Bağlı kullanıcı varsa silme işlemini engelleyip
        /// kullanıcıyı hata mesajıyla uyarır (veri bütünlüğü koruması).
        /// </summary>
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
        */
    }
}