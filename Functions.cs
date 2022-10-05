using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGatewayApi.Models
{
    public class Transactions
    {
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

        [Column(TypeName = "DateTime")]
        public DateTime BeginTransaction   { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Status { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime FinishTransaction { get; set; }

        [Column(TypeName = "int")]
        public int Userid { get; set; }

        [ForeignKey("UserId")]
        public virtual Users UserTrans { get; set; }
    }
    public class Transfers
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int TransferId { get; set; }

        [Column(TypeName = "int")]
        public int Amount { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string TypeTranfer{ get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime TransferDate { get; set; }

        [Column(TypeName = "int")]
        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public virtual Transactions TransTransfers { get; set; }

        [Column(TypeName = "int")]
        public int Userid { get; set; }

        [ForeignKey("UserId")]
        public virtual Users UserTransfers { get; set; }

    }

    public class Paymethods
        {
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

    }
    



    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //public int BookId { get; set; }

    //[Required]
    //[Column(TypeName = "nvarchar")]
    //[StringLength(500)]
    //public string Name { get; set; }

    //[Required]
    //[Column(TypeName = "nvarchar")]
    //[StringLength(100)]
    //public string Author { get; set; }

    //[Required]
    //[Column(TypeName = "bit")]
    //public bool Onlend { get; set; }

    //[Required]
    //[Column(TypeName = "int")]
    //public int YearPub { get; set; }

    //[Column(TypeName = "DateTime")]
    //public DateTime DateLend { get; set; }
}
