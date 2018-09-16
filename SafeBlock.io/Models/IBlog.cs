using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeBlock.io.Models
{
    public interface IBlog
    {
        List<Article> GetAllArticles();
        List<Article> GetArticlesByCategory(string category);
        List<Article> GetArticlesByTag(string tag);
        Article GetArticleBySeo(string seoTitle);
        int GetArticleNumberByCategory(string category);
        List<string> GetAllTags();
    }
}
