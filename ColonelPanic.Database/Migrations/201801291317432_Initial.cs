namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        ConfigID = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        GithubToken = c.String(),
                        LastGithubCommit = c.String(),
                        DestinyAPIToken = c.String(),
                    })
                .PrimaryKey(t => t.ConfigID);
            
            CreateTable(
                "dbo.GuildStates",
                c => new
                    {
                        GuildNum = c.Int(nullable: false, identity: true),
                        GuildId = c.String(),
                        GuildName = c.String(),
                        ScrumEnabled = c.Boolean(nullable: false),
                        NoteEnabled = c.Boolean(nullable: false),
                        PingGroupEnabled = c.Boolean(nullable: false),
                        CanSpeak = c.Boolean(nullable: false),
                        CanListen = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GuildNum);
            
            CreateTable(
                "dbo.TrustedUsers",
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
            DropTable("dbo.TrustedUsers");
            DropTable("dbo.GuildStates");
            DropTable("dbo.Configuration");
        }
    }
}
