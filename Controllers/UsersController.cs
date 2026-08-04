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

        /// <summary>
        /// Silinmemiş tüm kullanıcıları, ilişkili yönetici ve departman bilgileriyle birlikte
        /// isme göre sıralı şekilde çekip Kullanıcı Yönetimi sayfasını (Index view) döndürür.
        /// Arama/filtreleme/sayfalama işlemleri istemci tarafında (DataTables) yapılır.
        /// </summary>
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
        
       
        /// <summary>
        /// Formdan gönderilen bilgilerle yeni bir kullanıcı oluşturur.
        /// Model doğrulaması başarısız olursa hata mesajıyla listeleme sayfasına yönlendirir.
        /// Oluşturulma tarihini otomatik atar; varsa yüklenen profil fotoğrafını
        /// sunucuya kaydedip dosya yolunu veritabanına yazar.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(User user, IFormFile? ProfilePhoto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Index");
            }
            user.CreatedAt = DateTime.Now;

            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhoto.FileName);
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var savePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await ProfilePhoto.CopyToAsync(stream);
                }
                user.ProfilePhotoPath = fileName;
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            TempData["SuccessMessage"] = $"{user.Name} {user.Surname} başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Var olan bir kullanıcı kaydını, formdan gelen güncellenmiş bilgilerle günceller.
        /// Sadece formda yer alan alanlar değiştirilir; CreatedAt ve IsDeleted gibi
        /// formda bulunmayan alanlara dokunulmaz. Yeni bir fotoğraf seçilmediyse
        /// mevcut profil fotoğrafı korunur.
        /// </summary>
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

                if (ProfilePhoto != null && ProfilePhoto.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhoto.FileName);
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "profiles");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var savePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ProfilePhoto.CopyToAsync(stream);
                    }
                    existing.ProfilePhotoPath = fileName;
                }
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kullanıcı bilgileri başarıyla güncellendi.";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen kullanıcıyı veritabanından kalıcı olarak silmek yerine
        /// "silinmiş" olarak işaretler (soft delete). Böylece veri kaybı yaşanmadan
        /// kullanıcı normal listeden gizlenir ve gerektiğinde geri getirilebilir.
        /// </summary>
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

        /// <summary>
        /// Soft delete ile silinmiş (IsDeleted = true) kullanıcıları listeler.
        /// Bu sayfa üzerinden silinen kullanıcılar geri getirilebilir.
        /// </summary>
        public IActionResult Deleted()
        {
            var deletedUsers = _context.Users
                .Where(u => u.IsDeleted)
                .Include(u => u.Manager)
                .Include(u => u.Department)
                .ToList();
            return View(deletedUsers);
        }

        /// <summary>
        /// Daha önce soft delete ile silinmiş bir kullanıcının IsDeleted durumunu
        /// false yaparak onu tekrar aktif kullanıcı listesine dahil eder.
        /// </summary>
        [HttpPost]
        public IActionResult Restore(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsDeleted = false;
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"{user.Name} {user.Surname} geri getirildi.";
            }
            return RedirectToAction("Deleted");
        }

    }
}
