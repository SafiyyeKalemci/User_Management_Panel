using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using UserManagementSystem.Models.Dtos;

namespace UserManagementSystem.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class 
        
        UsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name,
            IsActive = user.IsActive,
            IsManager = user.IsManager,
            ManagerId = user.ManagerId,
            ManagerFullName = user.Manager != null ? $"{user.Manager.Name} {user.Manager.Surname}" : null,
            CreatedAt = user.CreatedAt
        };

        /// <summary>
        /// Tüm aktif (silinmemiş) kullanıcıları listeler.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Department)
                .Include(u => u.Manager)
                .ToListAsync();

            return Ok(users.Select(ToDto));
        }

        /// <summary>
        /// Id'ye göre tek bir kullanıcı getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Manager)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return NotFound(new { message = $"Id'si {id} olan kullanıcı bulunamadı." });

            return Ok(ToDto(user));
        }

        /// <summary>
        /// Yeni bir kullanıcı oluşturur.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                DepartmentId = dto.DepartmentId,
                ManagerId = dto.ManagerId,
                IsManager = dto.IsManager,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Department).LoadAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ToDto(user));
        }

        /// <summary>
        /// Var olan bir kullanıcıyı günceller.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return NotFound(new { message = $"Id'si {id} olan kullanıcı bulunamadı." });

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.DepartmentId = dto.DepartmentId;
            user.ManagerId = dto.ManagerId;
            user.IsManager = dto.IsManager;
            user.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            await _context.Entry(user).Reference(u => u.Department).LoadAsync();

            return Ok(ToDto(user));
        }

        /// <summary>
        /// Kullanıcıyı soft delete yapar (IsDeleted = true).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return NotFound(new { message = $"Id'si {id} olan kullanıcı bulunamadı." });

            user.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}