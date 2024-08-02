using Microsoft.AspNetCore.Identity;

namespace Project_API.Models
{
    public class AspNetUserRole : IdentityUserRole<int>
    {
        public virtual AspNetUser User { get; set; }
        public virtual AspNetRole Role { get; set; }
    }
}
