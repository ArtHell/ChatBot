namespace MyBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Line = c.Int(nullable: false),
                        Column = c.Int(nullable: false),
                        MyAliveCells = c.Int(nullable: false),
                        EnemyAliveCells = c.Int(nullable: false),
                        EnemyLine = c.Int(nullable: false),
                        EnemyColumn = c.Int(nullable: false),
                        GameStarted = c.Boolean(nullable: false),
                        MyField = c.String(),
                        EnemyField = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GameInfoes");
        }
    }
}
