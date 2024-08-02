using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Project_API.Models
{
    public partial class AspNetUser : IdentityUser<int>
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            UserDetails = new HashSet<UserDetail>();
            Roles = new HashSet<AspNetRole>();
            UserRoles = new HashSet<AspNetUserRole>();
        }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Discriminator { get; set; } = "AspNetUser";

        public byte Status {  get; set; }

        public string Avatar { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
        public virtual ICollection<AspNetRole> Roles { get; set; }
        public virtual ICollection<AspNetUserRole> UserRoles { get; set; }
    }
}
