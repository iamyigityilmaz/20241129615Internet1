using HaberPortali2.Models;
using System.Collections.Generic; // ICollection kullanımı için

namespace HaberPortali2.Models
{
    // Category: Haberlerin bağlı olduğu kategori tablosunu temsil eder
    public class Category
    {
        public int Id { get; set; } // Kategorinin birincil anahtar (Primary Key) alanı

        public string Name { get; set; } // Kategori adı (Örn: Spor, Gündem, Magazin)

        public bool IsActive { get; set; } // Kategori aktif / pasif durumu

        // Navigation Property:
        // Bir kategoride birden fazla haber olabilir.
        public ICollection<News>? NewsList { get; set; }
    }
}
