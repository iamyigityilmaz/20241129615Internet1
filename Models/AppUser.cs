using Microsoft.AspNetCore.Identity;

namespace HaberPortali2.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
