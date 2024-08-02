using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class UserDetail
    {
        public int UserDetailsId { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
