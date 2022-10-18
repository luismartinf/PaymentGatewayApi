using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PaymentGatewayApi.PaymentModels
{
    public class Users
    {
        public Users( )
        {
            UserId = 0;
            UserName = string.Empty;
            Name = string.Empty;
            Password = string.Empty;
            PhoneNumber = 0;
            URL = string.Empty;
            AddDate = DateTime.UnixEpoch;
            UserRoles = new List<Roles>(); 
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
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int PhoneNumber { get; set; }

#nullable enable
        [Column(TypeName = "nvarchar")]
        public string? URL { get; set; }
#nullable disable

        [Required]
        [Column(TypeName = "DateTime")]
        public DateTime AddDate { get; set; }
        public virtual ICollection<Roles> UserRoles { get; set; }
    }
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolesId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string RoleName { get; set; }
        public virtual ICollection<Users> UsersRole { get; set; }

    }

    public class UsersRoles
    {
        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        [Key, Column(Order = 2)]
        public int RoleId { get; set; }

        public Users User { get; set; }
        public Roles Role { get; set; }
    }

    public class UserLogins
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        
        public UserLogins() { }
    }
}
