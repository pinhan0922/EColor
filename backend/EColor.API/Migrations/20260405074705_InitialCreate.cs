using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EColor.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FabricType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaleStyleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FemaleStyleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DetailStyleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThreadCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Composition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CuttingDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionOrderId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionOrderItems_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrderItems_ProductionOrderId",
                table: "ProductionOrderItems",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_OrderNo",
                table: "ProductionOrders",
                column: "OrderNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionOrderItems");

            migrationBuilder.DropTable(
                name: "ProductionOrders");
        }
    }
}
