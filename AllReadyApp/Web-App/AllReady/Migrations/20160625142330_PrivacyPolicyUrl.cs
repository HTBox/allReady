using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class PrivacyPolicyUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivacyPolicyUrl",
                table: "Organization",
                nullable: true);          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropColumn(name: "PrivacyPolicyUrl", table: "Organization");          
        }
    }
}
