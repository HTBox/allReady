using Microsoft.EntityFrameworkCore.Migrations;

namespace AllReady.Migrations
{
    public partial class RenameImpactToGoal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CampaignImpact",
                newName: "CampaignGoal");

            migrationBuilder.RenameColumn(
                name: "CurrentImpactLevel",
                table: "CampaignGoal",
                newName: "CurrentGoalLevel");

            migrationBuilder.RenameColumn(
                name: "ImpactType",
                table: "CampaignGoal",
                newName: "GoalType");

            migrationBuilder.RenameColumn(
                name: "NumericImpactGoal",
                table: "CampaignGoal",
                newName: "NumericGoal");

            migrationBuilder.RenameColumn(
                name: "TextualImpactGoal",
                table: "CampaignGoal",
                newName: "TextualGoal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CampaignGoal",
                newName: "CampaignImpact");

            migrationBuilder.RenameColumn(
                newName: "CurrentImpactLevel",
                table: "CampaignGoal",
                name: "CurrentGoalLevel");

            migrationBuilder.RenameColumn(
                newName: "ImpactType",
                table: "CampaignGoal",
                name: "GoalType");

            migrationBuilder.RenameColumn(
                newName: "NumericImpactGoal",
                table: "CampaignGoal",
                name: "NumericGoal");

            migrationBuilder.RenameColumn(
                newName: "TextualImpactGoal",
                table: "CampaignGoal",
                name: "TextualGoal");
        }
    }
}
