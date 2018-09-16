using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeBlock.io.Models
{
    public interface IBlogs
    {
        List<Article> GetAllArticles();
    }
}
