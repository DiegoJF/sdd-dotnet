using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtorApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakePropertyImageUrlNullableForCreateProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "properties",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(260)",
                oldMaxLength: 260);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "properties",
                type: "character varying(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(260)",
                oldMaxLength: 260,
                oldNullable: true);
        }
    }
}
