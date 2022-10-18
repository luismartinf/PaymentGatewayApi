namespace PaymentGatewayApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Paymethods",
                c => new
                    {
                        PaymethodId = c.Int(nullable: false, identity: true),
                        TypePayment = c.String(nullable: false, maxLength: 4000),
                        PaymentNum = c.Int(nullable: false),
                        BillingAdress = c.String(nullable: false, maxLength: 500),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymethodId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 4000),
                        Password = c.String(nullable: false, maxLength: 4000),
                        PhoneNumber = c.Int(nullable: false),
                        URL = c.String(maxLength: 4000),
                        AddDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserName, unique: true);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RolesId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 4000),
                    })
                .PrimaryKey(t => t.RolesId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        TypeTransaction = c.Boolean(nullable: false),
                        Item = c.String(nullable: false, maxLength: 4000),
                        ShippingOrder = c.Int(nullable: false),
                        BeginTransaction = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 4000),
                        FinishTransaction = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        PaymethodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Paymethods", t => t.PaymethodId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.PaymethodId);
            
            CreateTable(
                "dbo.Transfers",
                c => new
                    {
                        TransferId = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        TypeTranfer = c.String(nullable: false, maxLength: 4000),
                        TransferDate = c.DateTime(nullable: false),
                        TransactionId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TransferId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.TransactionId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RolesUsers",
                c => new
                    {
                        Roles_RolesId = c.Int(nullable: false),
                        Users_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Roles_RolesId, t.Users_UserId })
                .ForeignKey("dbo.Roles", t => t.Roles_RolesId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Users_UserId, cascadeDelete: true)
                .Index(t => t.Roles_RolesId)
                .Index(t => t.Users_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transfers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Transfers", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "PaymethodId", "dbo.Paymethods");
            DropForeignKey("dbo.Paymethods", "UserId", "dbo.Users");
            DropForeignKey("dbo.RolesUsers", "Users_UserId", "dbo.Users");
            DropForeignKey("dbo.RolesUsers", "Roles_RolesId", "dbo.Roles");
            DropIndex("dbo.RolesUsers", new[] { "Users_UserId" });
            DropIndex("dbo.RolesUsers", new[] { "Roles_RolesId" });
            DropIndex("dbo.Transfers", new[] { "UserId" });
            DropIndex("dbo.Transfers", new[] { "TransactionId" });
            DropIndex("dbo.Transactions", new[] { "PaymethodId" });
            DropIndex("dbo.Transactions", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "UserName" });
            DropIndex("dbo.Paymethods", new[] { "UserId" });
            DropTable("dbo.RolesUsers");
            DropTable("dbo.Transfers");
            DropTable("dbo.Transactions");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.Paymethods");
        }
    }
}
