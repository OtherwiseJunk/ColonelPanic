namespace ColonelPanic.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserStatsProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserStates", "TableFlipPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserStates", "TableFlipPoints");
        }
    }
}
