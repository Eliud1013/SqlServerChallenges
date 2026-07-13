using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServerChallenges.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddChallengeSolutionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SolutionQuery",
                schema: "Challenges",
                table: "Challenges");

            migrationBuilder.CreateTable(
                name: "ChallengeSolutions",
                schema: "Challenges",
                columns: table => new
                {
                    ChallengeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatabaseProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SolutionSql = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeSolutions", x => x.ChallengeId);
                    table.ForeignKey(
                        name: "FK_ChallengeSolutions_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalSchema: "Challenges",
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeSolutions",
                schema: "Challenges");

            migrationBuilder.AddColumn<string>(
                name: "SolutionQuery",
                schema: "Challenges",
                table: "Challenges",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
