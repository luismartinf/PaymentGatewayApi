using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace PaymentGatewayApi.PaymentModels
{
    public class Users
    {
        public Users( )
        {
            UserId = 0;
            UserName = string.Empty;
            EmailAddress = string.Empty;
            Name = string.Empty;
            Password = string.Empty;
            PhoneNumber = 0;
            URL = null;
            AddDate = DateTime.UnixEpoch;
            Roles = new List<Roles>(); 
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string EmailAddress { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "bigint")]
        public long PhoneNumber { get; set; }

#nullable enable
        [Column(TypeName = "nvarchar")]
        public string? URL { get; set; }
#nullable disable

        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime AddDate { get; set; }
        public virtual ICollection<Roles> Roles { get; set; }
    }
    public class Roles
    {
        public Roles()
        {
            RolesId = 0;
            RoleName = string.Empty;
            Users = new List<Users>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolesId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string RoleName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Users> Users { get; set; }

    }

    //public class UsersRoles
    //{
    //    [Key, Column(Order = 1)]
    //    public int UserId { get; set; }

    //    [Key, Column(Order = 2)]
    //    public int RoleId { get; set; }

    //    public Users User { get; set; }
    //    public Roles Role { get; set; }
    //}

    public class UserLogins
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        
        public UserLogins() { }
    }
}
