namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveServerPerm : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ChannelStates", "ServerModuleEnabled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChannelStates", "ServerModuleEnabled", c => c.Boolean(nullable: false));
        }
    }
}
