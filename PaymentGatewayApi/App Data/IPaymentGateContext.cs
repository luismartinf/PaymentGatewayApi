using PaymentGatewayApi.PaymentModels;
using System.Data.Entity;

namespace PaymentGatewayApi.App_Data
{
    public interface IPaymentGateContext
    {
        DbSet<Users> Users { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<Transactions> Transactions { get; set; }
        DbSet<Transfers> Transfers { get; set; }
        DbSet<Paymethods> Paymethods { get; set; }
        int SaveChanges();

        void MarkAsModified(Object item)
        {

        }

        virtual bool RolesExists(int id)
        {
            return true;
        }
        virtual bool TransactionExists(int id)
        {
            return true;
        }
        virtual bool TransferExists(int id)
        {
            return true;
        }
        virtual bool PaymethodExists(int id)
        {
            return true;
        }        
        virtual Users? Findusertoken(string username)
        { return null; }

        virtual List<Roles> Rolestolist()
        { return new List<Roles>(); }

        virtual bool Delusersinroles(Roles role, Users user)
        {
            return true;
        }

        virtual bool Anyexceprole(Users firstuser, Users seconduser)
        {
             return true;
        }
        public virtual void Addusersinroles(Roles role, Users user)
        {
            
        }
    }
}