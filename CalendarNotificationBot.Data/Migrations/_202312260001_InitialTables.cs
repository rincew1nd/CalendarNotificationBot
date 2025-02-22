using FluentMigrator;

namespace CalendarNotificationBot.Data.Migrations;

[Migration(202502230001)]
public class _202502230001_InitialTables : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("ChatId").AsInt64()
            .WithColumn("UserName").AsString().Nullable()
            .WithColumn("FirstName").AsString().Nullable()
            .WithColumn("LastName").AsString().Nullable()
            .WithColumn("TimeZone").AsInt16().WithDefaultValue(0)
            .WithColumn("Culture").AsString().NotNullable().WithDefaultValue("en-US")
            .WithColumn("NotificationTime").AsByte().NotNullable().WithDefaultValue(15);

        Create.Table("calendars")
            .WithColumn("UserId").AsGuid().PrimaryKey().ForeignKey("IX_userId", "users", "Id")
            .WithColumn("CalendarUrl").AsString().Nullable()
            .WithColumn("CreationDate").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("ModificationDate").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("BitrixUserId").AsString().Nullable();
    }
    
    public override void Down()
    {
        Delete.Table("Calendar");
        Delete.Table("User");
    }
}