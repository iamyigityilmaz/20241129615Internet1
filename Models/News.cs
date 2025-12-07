using System;
using System.Collections.Generic;

namespace HaberPortali2.Models
{
    public class News
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        // TASLAK SİSTEMİ İÇİN KOLON DURUYOR AMA
        // ARTIK HER ZAMAN TRUE KULLANACAĞIZ
        public bool IsPublished { get; set; } = true;

        // KATEGORİ İLİŞKİSİ
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // HABERİ OLUŞTURAN KULLANICI (opsiyonel)
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        // HABERE AİT YORUMLAR
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
