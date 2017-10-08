namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChannelStates", "NoteEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChannelStates", "NoteEnabled");
        }
    }
}
