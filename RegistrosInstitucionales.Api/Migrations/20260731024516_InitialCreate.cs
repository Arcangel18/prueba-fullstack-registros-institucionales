using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistrosInstitucionales.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicioConvenio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinConvenio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuotaDiaria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogAccesos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoConsulta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdentificadorConsultado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAccesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogAccesos_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entidades_ApiKey",
                table: "Entidades",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogAccesos_EntidadId_FechaHora_Resultado",
                table: "LogAccesos",
                columns: new[] { "EntidadId", "FechaHora", "Resultado" });

            migrationBuilder.CreateIndex(
                name: "IX_Registros_Identificador_Nombre",
                table: "Registros",
                columns: new[] { "Identificador", "Nombre" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogAccesos");

            migrationBuilder.DropTable(
                name: "Registros");

            migrationBuilder.DropTable(
                name: "Entidades");
        }
    }
}
