using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyWithChatgpt.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_PostLike_UserId",
                table: "PostLike");

            // Repair data written by earlier versions before the schema tightens.
            migrationBuilder.Sql(@"
UPDATE BlogPosts
SET ImageUrl = NULL
WHERE ImageUrl IS NOT NULL
  AND LOWER(ImageUrl) NOT LIKE '%.jpg'
  AND LOWER(ImageUrl) NOT LIKE '%.jpeg'
  AND LOWER(ImageUrl) NOT LIKE '%.png'
  AND LOWER(ImageUrl) NOT LIKE '%.gif'
  AND LOWER(ImageUrl) NOT LIKE '%.webp';

;WITH duplicates AS (
    SELECT Id, ROW_NUMBER() OVER (PARTITION BY UserId, BlogPostId ORDER BY Id) AS rn
    FROM PostLike
)
DELETE FROM duplicates WHERE rn > 1;

UPDATE Comments SET Content = LEFT(Content, 1000) WHERE LEN(Content) > 1000;
UPDATE Comments SET Author = LEFT(Author, 80) WHERE LEN(Author) > 80;

;WITH duplicateNames AS (
    SELECT Id, Name, ROW_NUMBER() OVER (PARTITION BY Name ORDER BY Id) AS rn
    FROM Categories
)
UPDATE duplicateNames
SET Name = LEFT(Name, 48) + ' ' + CAST(Id AS nvarchar(10))
WHERE rn > 1;
");

            migrationBuilder.DropColumn(
                name: "IsLikedByCurrentUser",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Comments",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE c
SET c.UserId = u.Id
FROM Comments c
INNER JOIN AspNetUsers u ON LOWER(c.Author) = LOWER(u.UserName)
WHERE c.UserId IS NULL;
");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "BlogPosts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WebsiteUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LinkedInUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PostLike_UserId_BlogPostId",
                table: "PostLike",
                columns: new[] { "UserId", "BlogPostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CreatedAt",
                table: "BlogPosts",
                column: "CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_PostLike_UserId_BlogPostId",
                table: "PostLike");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_CreatedAt",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLikedByCurrentUser",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "WebsiteUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LinkedInUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostLike_UserId",
                table: "PostLike",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Categories_CategoryID",
                table: "BlogPosts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
