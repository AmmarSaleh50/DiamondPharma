using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondPharma.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyIdToMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PharmacyId",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_PharmacyId",
                table: "Medicines",
                column: "PharmacyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Medicines_PharmacyId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Medicines");
        }
    }
}
