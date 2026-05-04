using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiReviewHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAnalysisStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiAnalysisError",
                table: "Feedbacks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiAnalysisStatus",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiAnalysisError",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AiAnalysisStatus",
                table: "Feedbacks");
        }
    }
}
