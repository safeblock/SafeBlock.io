using System;

namespace SafeBlock.io.Models
{
    public class SupportArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SeoTitle { get; set; }
        public string Category { get; set; }
        public DateTime WriteDate { get; set; }
        public string Content { get; set; }
        public int ViewsNumber { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
