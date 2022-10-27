namespace PaymentGatewayApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WithoutDataAnnotations1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmailAddress", c => c.String(nullable: false, maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmailAddress");
        }
    }
}
