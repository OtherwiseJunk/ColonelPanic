namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNaughtyreduxThree : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserXChannelFlags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChannelId = c.String(),
                        UserId = c.String(),
                        Shitlist = c.Boolean(nullable: false),
                        EggplantList = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserStates",
                c => new
                    {
                        UserNum = c.Int(nullable: false, identity: true),
                        TableFlipPoints = c.Int(nullable: false),
                        IsNaughty = c.Boolean(nullable: false),
                        UserId = c.String(),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.UserNum);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserStates");
            DropTable("dbo.UserXChannelFlags");
        }
    }
}
