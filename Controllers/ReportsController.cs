using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;

namespace UserManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Raporlar sayfası için gerekli tüm istatistikleri hesaplar:
        /// aktif/pasif kullanıcı oranı, yöneticiye göre ekip büyüklüğü,
        /// en kalabalık/en az kalabalık departman ve organizasyon şeması
        /// için departman-yönetici-çalışan verisini hazırlar.
        /// Silinmiş (IsDeleted) kullanıcılar tüm hesaplamalardan hariç tutulur.
        /// </summary>
        public IActionResult Index()
        {
            var departments = _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users.Where(u => !u.IsDeleted))
                .ToList();

            var allUsers = _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Manager)
                .Include(u => u.Department)
                .ToList();

            // Aktif/Pasif oranı
            ViewBag.ActiveCount = allUsers.Count(u => u.IsActive);
            ViewBag.PassiveCount = allUsers.Count(u => !u.IsActive);

            // Yöneticiye göre ekip büyüklüğü
            var managers = allUsers.Where(u => u.IsManager).ToList();
            var teamSizes = managers
                .Select(m => new
                {
                    ManagerName = $"{m.Name} {m.Surname}",
                    TeamSize = allUsers.Count(u => u.ManagerId == m.Id)
                })
                .OrderByDescending(x => x.TeamSize)
                .ToList();

            ViewBag.ManagerNames = teamSizes.Select(x => x.ManagerName).ToList();
            ViewBag.TeamSizes = teamSizes.Select(x => x.TeamSize).ToList();

            // En kalabalık / en az kalabalık departman
            var deptCounts = departments
                .Select(d => new { d.Name, Count = d.Users?.Count ?? 0 })
                .ToList();

            ViewBag.MostCrowded = deptCounts.OrderByDescending(x => x.Count).FirstOrDefault();
            ViewBag.LeastCrowded = deptCounts.OrderBy(x => x.Count).FirstOrDefault();

            // Departman bazlı organizasyon şeması için ham veri
            ViewBag.Departments = departments;

            return View();
        }
    }
}