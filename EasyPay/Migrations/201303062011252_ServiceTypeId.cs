namespace EasyPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceTypeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceOperators", "ServiceTypeId", c => c.Int());
            AddForeignKey("dbo.ServiceOperators", "ServiceTypeId", "dbo.ServiceTypes", "ServiceTypeId");
            CreateIndex("dbo.ServiceOperators", "ServiceTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ServiceOperators", new[] { "ServiceTypeId" });
            DropForeignKey("dbo.ServiceOperators", "ServiceTypeId", "dbo.ServiceTypes");
            DropColumn("dbo.ServiceOperators", "ServiceTypeId");
        }
    }
}
