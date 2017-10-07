namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChannelStates",
                c => new
                    {
                        ChannelNum = c.Int(nullable: false, identity: true),
                        ChannelID = c.String(),
                        ChannelName = c.String(),
                        ScrumEnabled = c.Boolean(nullable: false),
                        ServerModuleEnabled = c.Boolean(nullable: false),
                        CanSpeak = c.Boolean(nullable: false),
                        CanListen = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChannelNum);
            
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        ConfigID = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        GithubToken = c.String(),
                        LastGithubCommit = c.String(),
                    })
                .PrimaryKey(t => t.ConfigID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserNum = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.UserNum);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Configuration");
            DropTable("dbo.ChannelStates");
        }
    }
}
