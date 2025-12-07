using System;

namespace HaberPortali2.Models
{
    // Comment: Haberler altına yapılan kullanıcı yorumlarını temsil eder
    public class Comment
    {
        public int Id { get; set; }
        // Yorumun benzersiz kimliği (Primary Key)

        public string UserName { get; set; }
        // Yorumu yapan kişinin ismi (zorunlu - login sistemi yoksa kullanıcı kendi girer)

        public string Text { get; set; }
        // Yorum içerik metni (zorunlu)

        public DateTime CreatedDate { get; set; }
        // Yorumun oluşturulma tarihi (News detayda gösterilecek)

        // ---------------------------------------------------
        // NEWS FK (Bu yorum hangi habere yazıldı?)
        // ---------------------------------------------------
        public int NewsId { get; set; }
        // Yorumun bağlı olduğu haberin Id'si (Foreign Key)

        public News News { get; set; }
        // Navigation: EF Core bu sayede ilişkili haber kaydına erişebilir
    }
}
