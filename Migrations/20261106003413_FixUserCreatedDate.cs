using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyWithChatgpt.Migrations
{
    /// <inheritdoc />
    public partial class FixUserCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Accounts created before the profile update inherited DateTime.MinValue
            // (displayed as "Jan 1, 0001"). Repair them using their earliest article,
            // falling back to the project launch date.
            migrationBuilder.Sql("""
                UPDATE u
                SET u.CreatedDate = COALESCE(
                    (SELECT MIN(p.CreatedAt) FROM BlogPosts p WHERE p.UserId = u.Id),
                    CONVERT(datetime2, '2025-01-01T00:00:00')
                )
                FROM AspNetUsers u
                WHERE u.CreatedDate < CONVERT(datetime2, '2000-01-01T00:00:00');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
