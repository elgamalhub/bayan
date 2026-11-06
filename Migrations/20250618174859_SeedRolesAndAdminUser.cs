using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using StudyWithChatgpt.Models;

#nullable disable

namespace StudyWithChatgpt.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Roles
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
            { "1", "Admin", "ADMIN", Guid.NewGuid().ToString() },
            { "2", "User", "USER", Guid.NewGuid().ToString() }
                }
            );
            // Password hash for: Admin123!
            var hasher = new PasswordHasher<ApplicationUser>();
            var passwordHash = hasher.HashPassword(null, "Admin123!");

            // Seed Admin User
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[]
                {
            "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
            "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed",
            "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount"
                },
                values: new object[]
                {
            "10", "admin", "ADMIN", "admin@blog.com", "ADMIN@BLOG.COM", true,
            passwordHash, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false,
            false, true, 0
                }
            );

            // Assign Admin Role
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "10", "1" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData("AspNetUserRoles", "UserId", "10");
            migrationBuilder.DeleteData("AspNetUsers", "Id", "10");
            migrationBuilder.DeleteData("AspNetRoles", "Id", "1");
            migrationBuilder.DeleteData("AspNetRoles", "Id", "2");


        }
    }
}
