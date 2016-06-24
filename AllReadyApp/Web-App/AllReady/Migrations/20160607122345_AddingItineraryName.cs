using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class AddingItineraryName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Itinerary",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Name", table: "Itinerary");
        }
    }
}
