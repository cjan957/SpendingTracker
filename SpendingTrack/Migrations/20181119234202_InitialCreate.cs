using Microsoft.EntityFrameworkCore.Migrations;

namespace SpendingTrack.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpendingItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TripID = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Heading = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    ReceiptID = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendingItem", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpendingItem");
        }
    }
}
