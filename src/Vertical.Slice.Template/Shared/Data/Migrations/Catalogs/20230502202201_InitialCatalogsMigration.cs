using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertical.Slice.Template.Shared.Data.Migrations.Catalogs
{
    /// <inheritdoc />
    public partial class InitialCatalogsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "catalog");

            migrationBuilder.CreateTable(
                name: "products",
                schema: "catalog",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                        category_id = table.Column<Guid>(type: "uuid", nullable: false),
                        name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                        description = table.Column<string>(
                            type: "character varying(250)",
                            maxLength: 250,
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_products_id",
                schema: "catalog",
                table: "products",
                column: "id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "products", schema: "catalog");
        }
    }
}
