using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class OrgDescFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionHtml",
                table: "Organization",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Organization",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DescriptionHtml", table: "Organization");
            migrationBuilder.DropColumn(name: "Summary", table: "Organization");
        }
    }
}
