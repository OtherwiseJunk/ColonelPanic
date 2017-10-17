namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyChannelStatesToGuildStates : DbMigration
    {
        public override void Up()
        {
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
            
            DropTable("dbo.ChannelStates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ChannelStates",
                c => new
                    {
                        ChannelNum = c.Int(nullable: false, identity: true),
                        ChannelID = c.String(),
                        ChannelName = c.String(),
                        ScrumEnabled = c.Boolean(nullable: false),
                        NoteEnabled = c.Boolean(nullable: false),
                        CanSpeak = c.Boolean(nullable: false),
                        CanListen = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChannelNum);
            
            DropTable("dbo.GuildStates");
        }
    }
}
