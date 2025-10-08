using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaKing.Migrations
{
    /// <inheritdoc />
    public partial class ShopCartItemIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopCartItemIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopCartItemId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopCartItemIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopCartItemIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopCartItemIngredients_ShopCartItems_ShopCartItemId",
                        column: x => x.ShopCartItemId,
                        principalTable: "ShopCartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopCartItemIngredients_IngredientId",
                table: "ShopCartItemIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCartItemIngredients_ShopCartItemId",
                table: "ShopCartItemIngredients",
                column: "ShopCartItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopCartItemIngredients");
        }
    }
}
