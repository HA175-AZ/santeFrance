using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace santeFrance.Migrations
{
    /// <inheritdoc />
    public partial class AjoutAuthMedecin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereConnexion",
                table: "Medecins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotDePasse",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DerniereConnexion",
                table: "Medecins");

            migrationBuilder.DropColumn(
                name: "MotDePasse",
                table: "Medecins");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Medecins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
