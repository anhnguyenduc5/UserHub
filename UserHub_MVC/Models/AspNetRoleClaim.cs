using Microsoft.AspNetCore.Identity;

namespace Project_MVC.Models
{
    public partial class AspNetRoleClaim : IdentityRoleClaim<int>
    {
        public virtual AspNetRole Role { get; set; } = null!;
    }
}
