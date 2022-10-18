using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Transactions;

namespace PaymentGatewayApi.PaymentModels
{
    public class Transactions
    {
        public Transactions()
        {   TransactionId = 0;
            Amount = 0;
            TypeTransaction = false;
            Item = string.Empty;
            ShippingOrder = 0;
            BeginTransaction = DateTime.UnixEpoch;
            Status = string.Empty;
            FinishTransaction = DateTime.UnixEpoch;
            UserId = 0;
            UserTrans = new Users();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Amount { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool TypeTransaction { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Item { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int ShippingOrder { get; set; }

        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime BeginTransaction { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Status { get; set; }

        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime FinishTransaction { get; set; }

        [Column(TypeName = "int")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users UserTrans { get; set; }

        public int PaymethodId { get; set; }

        [ForeignKey("PaymethodId")]
        public virtual Paymethods PaymethodTrans { get; set; }
    }
    public class Transfers
    {
        public Transfers( )
        {
            TransferId = 0;
            Amount = 0;
            TypeTranfer = string.Empty;
            TransferDate = DateTime.UnixEpoch;
            TransactionId = 0;
            TransTransfers = new Transactions();
            UserId = 0;
            UserTransfers = new Users();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferId { get; set; }

        [Column(TypeName = "int")]
        public int Amount { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string TypeTranfer { get; set; }

        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime TransferDate { get; set; }

        [Column(TypeName = "int")]
        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public virtual Transactions TransTransfers { get; set; }

        [Column(TypeName = "int")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users UserTransfers { get; set; }

    }

    public class Paymethods
    {
        public Paymethods()
        {
            PaymethodId = 0;
            TypePayment = string.Empty;
            PaymentNum = 0;
            BillingAdress = string.Empty;
            UserId = 0;
            UserPaymethod = new Users();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymethodId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string TypePayment { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int PaymentNum { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(500)]
        public string BillingAdress { get; set; }

        [Column(TypeName = "int")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users UserPaymethod { get; set; }

    }
}
