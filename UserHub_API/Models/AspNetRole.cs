using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Project_API.Models
{
    public partial class AspNetRole : IdentityRole<int>
    {
        public AspNetRole()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaim>();
            Users = new HashSet<AspNetUser>();
            UserRoles = new HashSet<AspNetUserRole>();
        }

        public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual ICollection<AspNetUser> Users { get; set; }
        public virtual ICollection<AspNetUserRole> UserRoles { get; set; }
    }
}
