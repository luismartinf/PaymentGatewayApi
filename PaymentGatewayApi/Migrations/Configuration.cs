namespace PaymentGatewayApi.Migrations
{
    using PaymentGatewayApi.PaymentModels;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PaymentGatewayApi.App_Data.PaymentGatewayContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PaymentGatewayApi.App_Data.PaymentGatewayContext context)
        {
            IList<Roles> roles = new List<Roles>();

            roles.Add(new Roles()
            {
                RolesId = 1,
                RoleName = "SuperAdmin",
                Users = new List<Users>()
            });

            roles.Add(new Roles()
            {
                RolesId = 2,
                RoleName = "Admin",
                Users = new List<Users>()
            });

            roles.Add(new Roles()
            {
                RolesId = 3,
                RoleName = "Seller",
                Users = new List<Users>()
            });

            roles.Add(new Roles()
            {
                RolesId = 4,
                RoleName = "Customer",
                Users = new List<Users>()
            });


            foreach (var role in roles)
            { context.Roles.AddOrUpdate(role); }

            base.Seed(context);
        }
    }
}
