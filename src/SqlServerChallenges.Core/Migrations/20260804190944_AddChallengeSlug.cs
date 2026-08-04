using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerChallenges.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddChallengeSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "Challenges",
                table: "Challenges",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Slug",
                schema: "Challenges",
                table: "Challenges",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Challenges_Slug",
                schema: "Challenges",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "Challenges",
                table: "Challenges");
        }
    }
}
