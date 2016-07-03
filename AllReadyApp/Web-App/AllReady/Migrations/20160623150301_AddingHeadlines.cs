using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class AddingHeadlines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "TaskSignup",
                nullable: false);
            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "Event",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "Campaign",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Headline", table: "Event");
            migrationBuilder.DropColumn(name: "Headline", table: "Campaign");
            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "TaskSignup",
                nullable: true);
        }
    }
}