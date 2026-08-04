namespace UserManagementSystem.Models.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerFullName { get; set; }
        public int UserCount { get; set; }
    }
}
