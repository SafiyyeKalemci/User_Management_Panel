using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Department", "Email", "IsActive", "ManagerId", "Name", "Surname" },
                values: new object[,]
                {
                    { 2, "Sales", "andrew.fuller@company.com", true, null, "Andrew", "Fuller" },
                    { 1, "Sales", "nancy.davolio@company.com", true, 2, "Nancy", "Davolio" },
                    { 3, "Sales", "janet.leverling@company.com", true, 2, "Janet", "Leverling" },
                    { 4, "Sales", "margaret.peacock@company.com", true, 2, "Margaret", "Peacock" },
                    { 5, "Sales", "steven.buchanan@company.com", true, 2, "Steven", "Buchanan" },
                    { 8, "Sales", "laura.callahan@company.com", true, 2, "Laura", "Callahan" },
                    { 6, "Sales", "michael.suyama@company.com", true, 5, "Michael", "Suyama" },
                    { 7, "Sales", "robert.king@company.com", false, 5, "Robert", "King" },
                    { 9, "Sales", "anne.dodsworth@company.com", false, 5, "Anne", "Dodsworth" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagerId",
                table: "Users",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
