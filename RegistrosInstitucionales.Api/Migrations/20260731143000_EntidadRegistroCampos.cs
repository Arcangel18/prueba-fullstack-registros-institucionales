using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistrosInstitucionales.Api.Migrations
{
    /// <inheritdoc />
    public partial class EntidadRegistroCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Entidades",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "CorreoResponsable",
                table: "Entidades",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoAutorizacionRuta",
                table: "Entidades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnlaceTecnico",
                table: "Entidades",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificacionFiscal",
                table: "Entidades",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpPublica",
                table: "Entidades",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolucionHabilitanteRuta",
                table: "Entidades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entidades_IdentificacionFiscal",
                table: "Entidades",
                column: "IdentificacionFiscal",
                unique: true,
                filter: "[IdentificacionFiscal] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entidades_IdentificacionFiscal",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "CorreoResponsable",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "DocumentoAutorizacionRuta",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "EnlaceTecnico",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "IdentificacionFiscal",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "IpPublica",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "ResolucionHabilitanteRuta",
                table: "Entidades");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Entidades",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
