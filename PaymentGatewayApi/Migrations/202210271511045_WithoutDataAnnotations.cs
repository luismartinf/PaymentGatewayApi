namespace PaymentGatewayApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WithoutDataAnnotations : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "RolesId");
            DropColumn("dbo.Roles", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "RolesId", c => c.Int(nullable: false));
        }
    }
}
