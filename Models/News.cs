namespace HaberPortali2.Models
{
    public class News
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}
