using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace crossblog.Migrations
{
    public partial class Article_Index_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Articles_Title",
                table: "Articles",
                column: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_Title",
                table: "Articles");
        }
    }
}
