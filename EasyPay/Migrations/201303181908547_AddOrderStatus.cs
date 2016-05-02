namespace EasyPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderStatus : DbMigration
    {
		public override void Up()
		{
			AddColumn("dbo.Orders", "Status", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Orders", "Status");
		}
    }
}
