using Microsoft.Data.Entity.Migrations;

namespace AllReady.Migrations
{
    public partial class RequestToEventMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Lattitude", table: "Request");
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Request",
                nullable: true);
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Request",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddForeignKey(
                name: "FK_Request_Event_EventId",
                table: "Request",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropForeignKey(name: "FK_Request_Event_EventId", table: "Request");
            migrationBuilder.DropColumn(name: "EventId", table: "Request");
            migrationBuilder.DropColumn(name: "Latitude", table: "Request");
            migrationBuilder.AddColumn<double>(
                name: "Lattitude",
                table: "Request",
                nullable: false,
                defaultValue: 0);            
        }
    }
}
