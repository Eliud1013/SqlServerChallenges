using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerChallenges.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Submissions");

            migrationBuilder.RenameTable(
                name: "Submissions",
                newName: "Submissions",
                newSchema: "Submissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Submissions",
                table: "Submissions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                schema: "Submissions",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubmittedSql",
                schema: "Submissions",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId",
                schema: "Submissions",
                table: "Submissions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_UserId",
                schema: "Submissions",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Outcome",
                schema: "Submissions",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "SubmittedSql",
                schema: "Submissions",
                table: "Submissions");

            migrationBuilder.RenameTable(
                name: "Submissions",
                schema: "Submissions",
                newName: "Submissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
