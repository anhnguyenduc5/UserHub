using Microsoft.AspNetCore.Identity;

namespace Project_MVC.Models
{
    public partial class AspNetUserToken : IdentityUserToken<int>
    {
        public virtual AspNetUser User { get; set; } = null!;
    }
}
