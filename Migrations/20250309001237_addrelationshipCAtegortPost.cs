using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bayan.Migrations
{
    /// <inheritdoc />
    public partial class addrelationshipCAtegortPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CategoryID",
                table: "BlogPosts",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_CategoryID",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "BlogPosts");
        }
    }
}
