using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiamondPharma.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyIdToMedicine_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing FK and re-add without cascade to avoid multiple cascade paths
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Medicines_Pharmacies_PharmacyId",
            //     table: "Medicines");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Pharmacies_PharmacyId",
                table: "Medicines",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert FK to cascade on delete
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Medicines_Pharmacies_PharmacyId",
            //     table: "Medicines");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Pharmacies_PharmacyId",
                table: "Medicines",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
