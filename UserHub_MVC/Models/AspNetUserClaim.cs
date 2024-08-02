using Microsoft.AspNetCore.Identity;

namespace Project_MVC.Models
{
    public partial class AspNetUserClaim : IdentityUserClaim<int>
    {
        public virtual AspNetUser User { get; set; } = null!;
    }
}
