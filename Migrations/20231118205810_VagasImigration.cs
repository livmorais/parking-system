using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_dotnet.Migrations
{
    /// <inheritdoc />
    public partial class VagasImigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vaga_Clientes_ClienteId",
                table: "Vaga");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaga_Estacionamentos_EstacionamentoId",
                table: "Vaga");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vaga",
                table: "Vaga");

            migrationBuilder.RenameTable(
                name: "Vaga",
                newName: "Vagas");

            migrationBuilder.RenameIndex(
                name: "IX_Vaga_EstacionamentoId",
                table: "Vagas",
                newName: "IX_Vagas_EstacionamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Vaga_ClienteId",
                table: "Vagas",
                newName: "IX_Vagas_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vagas",
                table: "Vagas",
                column: "VagaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vagas_Clientes_ClienteId",
                table: "Vagas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vagas_Estacionamentos_EstacionamentoId",
                table: "Vagas",
                column: "EstacionamentoId",
                principalTable: "Estacionamentos",
                principalColumn: "EstacionamentoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vagas_Clientes_ClienteId",
                table: "Vagas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vagas_Estacionamentos_EstacionamentoId",
                table: "Vagas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vagas",
                table: "Vagas");

            migrationBuilder.RenameTable(
                name: "Vagas",
                newName: "Vaga");

            migrationBuilder.RenameIndex(
                name: "IX_Vagas_EstacionamentoId",
                table: "Vaga",
                newName: "IX_Vaga_EstacionamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Vagas_ClienteId",
                table: "Vaga",
                newName: "IX_Vaga_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vaga",
                table: "Vaga",
                column: "VagaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaga_Clientes_ClienteId",
                table: "Vaga",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaga_Estacionamentos_EstacionamentoId",
                table: "Vaga",
                column: "EstacionamentoId",
                principalTable: "Estacionamentos",
                principalColumn: "EstacionamentoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
