using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdofCompany",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdofCompany",
                table: "AspNetUsers",
                column: "IdofCompany");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CompanyList_IdofCompany",
                table: "AspNetUsers",
                column: "IdofCompany",
                principalTable: "CompanyList",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CompanyList_IdofCompany",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdofCompany",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdofCompany",
                table: "AspNetUsers");
        }
    }
}
