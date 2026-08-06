using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models.Dtos;

namespace UserManagementSystem.Controllers.Api
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Raporlar sayfasındaki tüm istatistikleri JSON olarak döner:
        /// aktif/pasif kullanıcı oranı, yöneticiye göre ekip büyüklüğü,
        /// en kalabalık/en az kalabalık departman ve organizasyon şeması verisi.
        /// Silinmiş (IsDeleted) kullanıcılar hesaplamalara dahil edilmez.
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ReportsSummaryDto>> GetSummary()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users.Where(u => !u.IsDeleted))
                .ToListAsync();

            var allUsers = await _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Manager)
                .ToListAsync();

            var managers = allUsers.Where(u => u.IsManager).ToList();
            var teamSizes = managers
                .Select(m => new ManagerTeamSizeDto
                {
                    ManagerName = $"{m.Name} {m.Surname}",
                    TeamSize = allUsers.Count(u => u.ManagerId == m.Id)
                })
                .OrderByDescending(x => x.TeamSize)
                .ToList();

            var deptCounts = departments
                .Select(d => new DepartmentCountDto { Name = d.Name, Count = d.Users?.Count ?? 0 })
                .ToList();

            var summary = new ReportsSummaryDto
            {
                ActiveCount = allUsers.Count(u => u.IsActive),
                PassiveCount = allUsers.Count(u => !u.IsActive),
                TeamSizes = teamSizes,
                MostCrowdedDepartment = deptCounts.OrderByDescending(x => x.Count).FirstOrDefault(),
                LeastCrowdedDepartment = deptCounts.OrderBy(x => x.Count).FirstOrDefault(),
                Departments = departments.Select(d => new DepartmentOrgDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerFullName = d.Manager != null ? $"{d.Manager.Name} {d.Manager.Surname}" : null,
                    Users = d.Users.Select(u => new OrgUserDto
                    {
                        Id = u.Id,
                        FullName = $"{u.Name} {u.Surname}",
                        IsManager = u.IsManager
                    }).ToList()
                }).ToList()
            };

            return Ok(summary);
        }
    }
}