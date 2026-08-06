namespace UserManagementSystem.Models.Dtos
{
    public class ReportsSummaryDto
    {
        public int ActiveCount { get; set; }
        public int PassiveCount { get; set; }
        public List<ManagerTeamSizeDto> TeamSizes { get; set; } = new();
        public DepartmentCountDto? MostCrowdedDepartment { get; set; }
        public DepartmentCountDto? LeastCrowdedDepartment { get; set; }
        public List<DepartmentOrgDto> Departments { get; set; } = new();
    }

    public class ManagerTeamSizeDto
    {
        public string ManagerName { get; set; } = string.Empty;
        public int TeamSize { get; set; }
    }

    public class DepartmentCountDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DepartmentOrgDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ManagerFullName { get; set; }
        public List<OrgUserDto> Users { get; set; } = new();
    }

    public class OrgUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsManager { get; set; }
    }
}