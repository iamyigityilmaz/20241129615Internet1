using System;

namespace HaberPortali2.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string FullName { get; set; } 

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; } 

        public bool IsAdmin { get; set; } 

    }
}
