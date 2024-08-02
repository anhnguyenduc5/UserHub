using Microsoft.AspNetCore.Identity;

namespace Project_MVC.Models
{
    public partial class AspNetUserLogin : IdentityUserLogin<int>
    {
        public virtual AspNetUser User { get; set; } = null!;
    }
}
