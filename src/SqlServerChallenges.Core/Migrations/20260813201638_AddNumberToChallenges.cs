using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerChallenges.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberToChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "Challenges",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Number",
                schema: "Challenges",
                table: "Challenges",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Challenges_Number",
                schema: "Challenges",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "Challenges",
                table: "Challenges");
        }
    }
}
