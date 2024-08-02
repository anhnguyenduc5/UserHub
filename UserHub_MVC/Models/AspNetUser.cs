using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class AspNetUser : IdentityUser<int>
    {
        public AspNetUser()
        {
            UserLogins = new HashSet<AspNetUserLogin>();
            UserTokens = new HashSet<AspNetUserToken>();
            UserClaims = new HashSet<AspNetUserClaim>();
            UserDetails = new HashSet<UserDetail>();
            Roles = new HashSet<AspNetRole>();
        }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Discriminator { get; set; } = "AspNetUser";

        public byte Status { get; set; }

        public string Avatar {  get; set; }

        public virtual ICollection<AspNetUserLogin> UserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> UserTokens { get; set; }
        public virtual ICollection<AspNetUserClaim> UserClaims { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
