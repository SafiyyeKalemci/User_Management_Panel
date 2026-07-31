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

        public IActionResult Index()
        {

            var users = _context.Users
                .Where(u => !u.IsDeleted)   
                .Include(u => u.Manager)
                .Include(u => u.Department)
                .OrderBy(u => u.Name)
                .ToList();

            // Dropdown ve istatistik kartları için TÜM kullanıcılar/departmanlar lazım (filtrelenmemiş)
            ViewBag.AllUsers = users;
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.LastAddedUser = users.OrderByDescending(u => u.CreatedAt).FirstOrDefault();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user, IFormFile? ProfilePhoto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Index");
            }
            user.CreatedAt = DateTime.Now;

            if(ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhoto.FileName);
                var savePath = Path.Combine("wwwroot/uploads/profiles", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await ProfilePhoto.CopyToAsync(stream);
                }
                user.ProfilePhotoPath = fileName;
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user, IFormFile? ProfilePhoto)
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

                if(ProfilePhoto != null && ProfilePhoto.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhoto.FileName);
                    var savePath = Path.Combine("wwwroot/uploads/profiles", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ProfilePhoto.CopyToAsync(stream);
                    }
                    existing.ProfilePhotoPath = fileName;
                }
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
                user.IsDeleted = true; // Soft delete
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Deleted()
        {
            var deletedUsers = _context.Users
                .Where(u => u.IsDeleted)
                .Include(u => u.Manager)
                .Include(u => u.Department)
                .ToList();
            return View(deletedUsers);
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsDeleted = false;
                _context.SaveChanges();
            }
            return RedirectToAction("Deleted");
        }

    }
}
