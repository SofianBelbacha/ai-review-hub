using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiReviewHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProAiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActionRequired",
                table: "Feedbacks",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyTopics",
                table: "Feedbacks",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityScore",
                table: "Feedbacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "Feedbacks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SentimentScore",
                table: "Feedbacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Urgency",
                table: "Feedbacks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionRequired",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "KeyTopics",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "PriorityScore",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "SentimentScore",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "Feedbacks");
        }
    }
}
