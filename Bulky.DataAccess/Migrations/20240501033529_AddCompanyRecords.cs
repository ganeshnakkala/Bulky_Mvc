using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "CompanyList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyList",
                table: "CompanyList",
                column: "Id");

            migrationBuilder.InsertData(
                table: "CompanyList",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Peoria", "TeckSol", "9517532486", "456123", "IL", "153 w markin" },
                    { 2, "Bloom", "CampSolutions", "4561239877", "425631", "WY", "56 w joom" },
                    { 3, "Peoria", "CrackTeck", "8459623215", "741258", "LD", "555 w spin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyList",
                table: "CompanyList");

            migrationBuilder.DeleteData(
                table: "CompanyList",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CompanyList",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CompanyList",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameTable(
                name: "CompanyList",
                newName: "Company");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");
        }
    }
}
