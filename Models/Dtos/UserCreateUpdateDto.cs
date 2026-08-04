using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models.Dtos
{
    public class UserCreateUpdateDto
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
        public int? ManagerId { get; set; }
        public bool IsManager { get; set; }
        public bool IsActive { get; set; }
    }
}