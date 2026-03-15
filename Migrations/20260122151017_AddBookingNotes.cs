using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunriseHotelApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS(SELECT * FROM sys.tables WHERE name = 'Customers')
                BEGIN
                    IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND name = 'IdentityCard')
                    BEGIN
                        ALTER TABLE [dbo].[Customers] ADD [IdentityCard] nvarchar(max) NULL;
                    END
                END
            ");

           
            migrationBuilder.Sql(@"
                IF EXISTS(SELECT * FROM sys.tables WHERE name = 'Bookings')
                BEGIN
                    IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND name = 'Notes')
                    BEGIN
                        ALTER TABLE [dbo].[Bookings] ADD [Notes] nvarchar(max) NULL;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}