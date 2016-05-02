namespace EasyPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResponseTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionResponses",
                c => new
                    {
                        TransactionResponseID = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ResponseCode = c.String(),
                        ResponseMessage = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        PaymentID = c.Int(nullable: false),
                        MerchantRefNo = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Mode = c.String(),
                        BillingName = c.String(),
                        TransactionID = c.Int(nullable: false),
                        IsFlagged = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionResponseID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactionResponses");
        }
    }
}
