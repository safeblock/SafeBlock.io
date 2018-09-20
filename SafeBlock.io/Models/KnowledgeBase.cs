using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeBlock.io.Models
{
    public class KnowledgeBase
    {
        [Required]
        public string Query { get; set; }
        public List<SupportArticle> Items { get; set; }
    }
}