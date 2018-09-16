using System;

namespace SafeBlock.io.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Title { get; set; }
        public string SeoTitle { get; set; }
        public string Tags { get; set; }
        public string Category { get; set; }
        public string Cover { get; set; }
        public int Author { get; set; }
        public DateTime? WriteDate { get; set; }
        public string Preamble { get; set; }
        public string Content { get; set; }
        public int ViewsNumber { get; set; }
    }
}
