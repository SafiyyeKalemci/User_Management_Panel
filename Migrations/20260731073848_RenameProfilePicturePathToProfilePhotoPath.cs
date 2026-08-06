using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameProfilePicturePathToProfilePhotoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.RenameColumn(
                name: "ProfilePicturePath",
                table: "Users",
                newName: "ProfilePhotoPath");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.RenameColumn(
                name: "ProfilePhotoPath",
                table: "Users",
                newName: "ProfilePicturePath");
            */
        }
    }
}
