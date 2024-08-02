using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class AspNetRole : IdentityRole<int>
    {
        public AspNetRole()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaim>();
            Users = new HashSet<AspNetUser>();
        }
        public string Discriminator { get; set; } = null!;

        public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual ICollection<AspNetUser> Users { get; set; }
    }
}
