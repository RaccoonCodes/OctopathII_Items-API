using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OctopathII_Items.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Max_Hp = table.Column<int>(type: "int", nullable: false),
                    Max_SP = table.Column<int>(type: "int", nullable: false),
                    Physical_Atk = table.Column<int>(type: "int", nullable: false),
                    Elemental_Atk = table.Column<int>(type: "int", nullable: false),
                    Physical_Def = table.Column<int>(type: "int", nullable: false),
                    Elemental_Def = table.Column<int>(type: "int", nullable: false),
                    Accuracy = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Critical = table.Column<int>(type: "int", nullable: false),
                    Evasion = table.Column<int>(type: "int", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    Buy_Price = table.Column<int>(type: "int", nullable: false),
                    Sell_Price = table.Column<int>(type: "int", nullable: false),
                    Equipment_Type = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Buy_Price = table.Column<int>(type: "int", nullable: false),
                    Sell_Price = table.Column<int>(type: "int", nullable: false),
                    Item_Type = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Elemental_Atk",
                table: "Equipment",
                column: "Elemental_Atk");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Equipment_Type",
                table: "Equipment",
                column: "Equipment_Type");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Physical_Atk",
                table: "Equipment",
                column: "Physical_Atk");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Sell_Price",
                table: "Equipment",
                column: "Sell_Price");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Item_Type",
                table: "Items",
                column: "Item_Type");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Sell_Price",
                table: "Items",
                column: "Sell_Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
