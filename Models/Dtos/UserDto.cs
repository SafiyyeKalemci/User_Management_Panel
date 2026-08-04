namespace UserManagementSystem.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; }
        public bool IsManager { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerFullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}