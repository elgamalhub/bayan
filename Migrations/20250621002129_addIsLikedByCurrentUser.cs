using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyWithChatgpt.Migrations
{
    /// <inheritdoc />
    public partial class addIsLikedByCurrentUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLikedByCurrentUser",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLikedByCurrentUser",
                table: "BlogPosts");
        }
    }
}
