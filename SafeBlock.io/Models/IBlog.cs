using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeBlock.io.Models
{
    public interface IBlog
    {
        List<Article> GetAllArticles();
    }
}
