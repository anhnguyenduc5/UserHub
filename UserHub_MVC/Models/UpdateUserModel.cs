namespace Project_MVC.Models
{
    public partial class UpdateUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? Role { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public IFormFile? AvatarImg { get; set; }
        public string? Avatar { get; set; } = string.Empty;
    }
}
