using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerChallenges.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChallengesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiredOrdering",
                schema: "Challenges",
                table: "Challenges",
                newName: "RequiresOrdering");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiresOrdering",
                schema: "Challenges",
                table: "Challenges",
                newName: "RequiredOrdering");
        }
    }
}
