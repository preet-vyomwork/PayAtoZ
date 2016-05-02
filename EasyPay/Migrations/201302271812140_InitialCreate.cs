namespace EasyPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProviderServices",
                c => new
                    {
                        ProviderServiceId = c.Int(nullable: false, identity: true),
                        ProviderId = c.Int(nullable: false),
                        ServiceTypeId = c.Int(nullable: false),
                        ServiceOperatorId = c.Int(nullable: false),
                        Comission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Format = c.String(),
                    })
                .PrimaryKey(t => t.ProviderServiceId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceOperators", t => t.ServiceOperatorId, cascadeDelete: true)
                .ForeignKey("dbo.Providers", t => t.ProviderId, cascadeDelete: true)
                .Index(t => t.ServiceTypeId)
                .Index(t => t.ServiceOperatorId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.ServiceTypes",
                c => new
                    {
                        ServiceTypeId = c.Int(nullable: false, identity: true),
                        ServiceTypeName = c.String(),
                    })
                .PrimaryKey(t => t.ServiceTypeId);
            
            CreateTable(
                "dbo.ServiceOperators",
                c => new
                    {
                        ServiceOperatorId = c.Int(nullable: false, identity: true),
                        OperatorName = c.String(),
                    })
                .PrimaryKey(t => t.ServiceOperatorId);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        ProviderId = c.Int(nullable: false, identity: true),
                        ProviderName = c.String(),
                    })
                .PrimaryKey(t => t.ProviderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.Int(nullable: false),
                        ProviderServiceId = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        ItemCode = c.String(),
                        Remarks = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId, cascadeDelete: true)
                .ForeignKey("dbo.ProviderServices", t => t.ProviderServiceId, cascadeDelete: true)
                .Index(t => t.UserProfileId)
                .Index(t => t.ProviderServiceId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserProfileId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Orders", new[] { "ProviderServiceId" });
            DropIndex("dbo.Orders", new[] { "UserProfileId" });
            DropIndex("dbo.ProviderServices", new[] { "ProviderId" });
            DropIndex("dbo.ProviderServices", new[] { "ServiceOperatorId" });
            DropIndex("dbo.ProviderServices", new[] { "ServiceTypeId" });
            DropForeignKey("dbo.Orders", "ProviderServiceId", "dbo.ProviderServices");
            DropForeignKey("dbo.Orders", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProviderServices", "ProviderId", "dbo.Providers");
            DropForeignKey("dbo.ProviderServices", "ServiceOperatorId", "dbo.ServiceOperators");
            DropForeignKey("dbo.ProviderServices", "ServiceTypeId", "dbo.ServiceTypes");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Orders");
            DropTable("dbo.Providers");
            DropTable("dbo.ServiceOperators");
            DropTable("dbo.ServiceTypes");
            DropTable("dbo.ProviderServices");
        }
    }
}
