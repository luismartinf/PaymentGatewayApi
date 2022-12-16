﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
            UpdateDate = DateTime.UnixEpoch;
            Roles = new List<Roles>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public virtual string UserName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string EmailAddress { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        public virtual string Password { get; set; }

        [Required]
        [Column(TypeName = "bigint")]
        public long PhoneNumber { get; set; }

#nullable enable
        [Column(TypeName = "nvarchar")]
        public string? URL { get; set; }
#nullable disable

        [Required]
        [Column(TypeName = "DateTime")]
        public virtual DateTime AddDate { get; set; }

        [Required]
        [Column(TypeName = "DateTime")]
        public virtual DateTime UpdateDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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

        public virtual ICollection<Users> Users { get; set; }

    }

    public class UserTokens
    {
        public string Token { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public TimeSpan Validaty { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public int UserId { get; set; }

        public Guid GuidId { get; set; }

        public DateTime ExpiredTime { get; set; }

        public Dictionary<int, string> Roles { get; set; } = new Dictionary<int, string>();

    }

    public class UserLogins
    {
    
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        
    }
}
