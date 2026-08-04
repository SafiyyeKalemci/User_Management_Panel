using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models.Dtos
{
    public class DepartmentUpdateDto
    {
        [Required(ErrorMessage = "Departman adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }
    }
}