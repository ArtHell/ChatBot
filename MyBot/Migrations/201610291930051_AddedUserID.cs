namespace MyBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameInfoes", "RecipientId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameInfoes", "RecipientId");
        }
    }
}
