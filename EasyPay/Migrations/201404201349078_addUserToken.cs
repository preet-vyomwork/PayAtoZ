namespace EasyPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "UserToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "UserToken");
        }
    }
}
