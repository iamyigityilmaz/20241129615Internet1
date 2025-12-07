using System;

namespace HaberPortali2.Models
{
    // Sistemdeki kullanıcıları temsil eder
    public class User
    {
        public int Id { get; set; } // Primary Key

        public string FullName { get; set; } // Kullanıcının adı-soyadı

        public string Email { get; set; } // Kullanıcının e-mail adresi

        public string PasswordHash { get; set; } // Şifrenin hashlenmiş hali

        public DateTime CreatedDate { get; set; } // Hesap oluşturulma tarihi

        public bool IsAdmin { get; set; } // Admin mi? Normal kullanıcı mı?

    }
}
