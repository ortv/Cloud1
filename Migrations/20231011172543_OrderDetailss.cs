using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud1.Migrations
{
    public partial class OrderDetailss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderDetailsId",
                table: "CartItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HebcalResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dayInAWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IfHoliday = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HebcalResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeelsLike = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    weatherResponseId = table.Column<int>(type: "int", nullable: false),
                    hebcalResponseId = table.Column<int>(type: "int", nullable: false),
                    orderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_HebcalResponse_hebcalResponseId",
                        column: x => x.hebcalResponseId,
                        principalTable: "HebcalResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Order_orderId",
                        column: x => x.orderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_WeatherResponse_weatherResponseId",
                        column: x => x.weatherResponseId,
                        principalTable: "WeatherResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_OrderDetailsId",
                table: "CartItem",
                column: "OrderDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_hebcalResponseId",
                table: "OrderDetails",
                column: "hebcalResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_orderId",
                table: "OrderDetails",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_weatherResponseId",
                table: "OrderDetails",
                column: "weatherResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_OrderDetails_OrderDetailsId",
                table: "CartItem",
                column: "OrderDetailsId",
                principalTable: "OrderDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_OrderDetails_OrderDetailsId",
                table: "CartItem");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "HebcalResponse");

            migrationBuilder.DropTable(
                name: "WeatherResponse");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_OrderDetailsId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "OrderDetailsId",
                table: "CartItem");
        }
    }
}
