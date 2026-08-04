using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using UserManagementSystem.Models.Dtos;

namespace UserManagementSystem.Controllers.Api
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static DepartmentDto ToDto(Department dept) => new DepartmentDto
        {
            Id = dept.Id,
            Name = dept.Name,
            ManagerId = dept.ManagerId,
            ManagerFullName = dept.Manager != null ? $"{dept.Manager.Name} {dept.Manager.Surname}" : null,
            UserCount = dept.Users?.Count ?? 0
        };

        /// <summary>
        /// Tüm departmanları listeler.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .ToListAsync();

            return Ok(departments.Select(ToDto));
        }

        /// <summary>
        /// Id'ye göre tek bir departman getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound(new { message = $"Id'si {id} olan departman bulunamadı." });

            return Ok(ToDto(department));
        }

        /// <summary>
        /// Var olan bir departmanı günceller.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound(new { message = $"Id'si {id} olan departman bulunamadı." });

            department.Name = dto.Name;
            department.ManagerId = dto.ManagerId;

            await _context.SaveChangesAsync();
            await _context.Entry(department).Reference(d => d.Manager).LoadAsync();

            return Ok(ToDto(department));
        }

        /// <summary>
        /// Departmanı siler. Departmanda kayıtlı kullanıcı varsa silme işlemi engellenir.
        /// </summary>
        /// // NOT: Departman silme MVC tarafında da kapatıldı — departmanlar kolay silinmemeli
        // (kullanıcı ataması, geçmiş veri bütünlüğü riski). İleride "pasife alma" gibi
        // bir soft-delete mantığı gerekirse tekrar değerlendirilebilir.
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound(new { message = $"Id'si {id} olan departman bulunamadı." });

            if (department.Users.Any())
                return BadRequest(new { message = "Bu departmanda kayıtlı kullanıcılar var, önce onları başka bir departmana taşıyın veya silin." });

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
    }
}