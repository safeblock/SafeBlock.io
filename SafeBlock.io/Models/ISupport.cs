using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeBlock.io.Models
{
    public interface ISupport
    {
        List<SupportArticle> GetAllArticles();
        List<SupportArticle> GetArticlesByTerm(string term);
        List<SupportArticle> GetArticlesByCategory(string term);
        SupportArticle GetArticleBySeo(string seoTitle);
    }
}
