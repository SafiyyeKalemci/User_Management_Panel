using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;

namespace UserManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, int? departmentId, string? status, string? sort, int page = 1, int pageSize = 10)
        {

            var query = _context.Users
                .Include(u => u.Manager)
                .Include(u => u.Department)
                .AsQueryable();

            // Arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Surname.Contains(search) ||
                    u.Email.Contains(search));
            }

            // Departman filtresi
            if (departmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == departmentId);
            }

            // Durum filtresi
            if (status == "active")
            {
                query = query.Where(u => u.IsActive);
            }
            else if (status == "passive")
            {
                query = query.Where(u => !u.IsActive);
            }

            // Sıralama
            query = sort == "desc"
                ? query.OrderByDescending(u => u.Name)
                : query.OrderBy(u => u.Name);

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Dropdown ve istatistik kartları için TÜM kullanıcılar/departmanlar lazım (filtrelenmemiş)
            ViewBag.AllUsers = _context.Users.ToList();
            ViewBag.Departments = _context.Departments.ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.DepartmentId = departmentId;
            ViewBag.Status = status;
            ViewBag.Sort = sort;

            return View(users);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Index");
            }
            user.CreatedAt = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Index");
            }
            var existing = _context.Users.Find(user.Id);
            if (existing != null)
            {
                existing.Name = user.Name;
                existing.Surname = user.Surname;
                existing.Email = user.Email;
                existing.DepartmentId = user.DepartmentId;
                existing.ManagerId = user.ManagerId;
                existing.IsActive = user.IsActive;
                existing.IsManager = user.IsManager;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
