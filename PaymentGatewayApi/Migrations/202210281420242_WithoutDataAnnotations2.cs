namespace PaymentGatewayApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WithoutDataAnnotations2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UpdateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UpdateDate");
        }
    }
}
