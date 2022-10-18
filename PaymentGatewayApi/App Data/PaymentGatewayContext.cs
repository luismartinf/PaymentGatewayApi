using static PaymentGatewayApi.App_Data.PaymentGatewayContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using PaymentGatewayApi.PaymentModels;

namespace PaymentGatewayApi.App_Data
{
    public class PaymentGatewayContext : DbContext, IPaymentGateContext
    {
        public PaymentGatewayContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<PaymentGatewayContext>(new UniDBInitializer<PaymentGatewayContext>());
        }

        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<Roles> Roles { get; set; } = null!;
        public virtual DbSet<Transactions> Transactions { get; set; } = null!;
        public virtual DbSet<Transfers> Transfers { get; set; } = null!;
        public virtual DbSet<Paymethods> Paymethods { get; set; } = null!;

        void MarkAsModified(Users item)
        {
            Entry(item).State = EntityState.Modified;
        }
        public void MarkAsModified(Roles item)
        {
            Entry(item).State = EntityState.Modified;
        }
        public void MarkAsModified(Transactions item)
        {
            Entry(item).State = EntityState.Modified;
        }
        public void MarkAsModified(Transfers item)
        {
            Entry(item).State = EntityState.Modified;
        }
        public void MarkAsModified(Paymethods item)
        {
            Entry(item).State = EntityState.Modified;
        }

        public virtual bool UsersExists(int id)
        {
            return Users.Any(e => e.UserId == id);
        }
        public virtual bool RolesExists(int id)
        {
            return Roles.Any(e => e.RolesId == id);
        }

        public virtual bool TransactionExists(int id)
        {
            return Transactions.Any(e => e.TransactionId == id);
        }
        public virtual bool TransferExists(int id)
        {
            return Transfers.Any(e => e.TransferId == id);
        }
        public virtual bool PaymethodExists(int id)
        {
            return Paymethods.Any(e => e.PaymethodId == id);
        }
        private class UniDBInitializer<T> : DropCreateDatabaseAlways<PaymentGatewayContext>
        {

            protected override void Seed(PaymentGatewayContext context)
            {

                IList<Roles> roles = new List<Roles>();

                roles.Add(new Roles()
                {
                    RoleName = "Admin",
                    UsersRole = new List<Users>()
                });

                roles.Add(new Roles()
                {
                    RoleName = "SuperAdmin",
                    UsersRole = new List<Users>()
                });

                roles.Add(new Roles()
                {
                    RoleName = "Customer",
                    UsersRole = new List<Users>()
                });

                roles.Add(new Roles()
                {
                    RoleName = "Seller",
                    UsersRole = new List<Users>()
                });


                foreach (Roles role in roles)
                    context.Roles.Add(role);
                base.Seed(context);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().Property(p => p.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Roles>().Property(p => p.RolesId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transactions>().Property(p => p.TransactionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transfers>().Property(p => p.TransferId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paymethods>().Property(p => p.PaymethodId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().HasIndex(u => u.UserName).IsUnique();
        }
    }

    public class LibraryContextFactory : IDbContextFactory<PaymentGatewayContext>
    {
        public LibraryContextFactory()
        {
        }

        public PaymentGatewayContext Create()
        {
            return new PaymentGatewayContext("Server=ASPLAPLTM095;Database=PaymentGatewayApiDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }


    }
}
    
           

