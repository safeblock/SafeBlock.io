using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeBlock.io.Models
{
    public class KnowledgeBase
    {
        [Required]
        public string Query { get; set; }
        public List<KnowledgeItem> Items { get; set; }
    }

    public class KnowledgeItem
    {
    }
}