using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Departman adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }
        public User? Manager { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}