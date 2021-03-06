using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SafeBlock.io.Models
{
    public class Support : ISupport
    {
        private readonly SafeBlockContext _context = null;

        public Support(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<SupportArticle> GetAllArticles()
        {
            return _context.Support.ToList();
        }

        public List<SupportArticle> GetArticlesByTerm(string term)
        {
            return _context.Support.Where(x => x.Title.Contains(term) || x.Content.Contains(term) || x.Category.Contains(term)).OrderByDescending(x => x.Id).ToList();
        }

        public List<SupportArticle> GetArticlesByCategory(string category)
        {
            return _context.Support.Where(x => x.Category.Equals(category)).OrderByDescending(x => x.Id).ToList();
        }

        public SupportArticle GetArticleBySeo(string seoTitle)
        {
            return _context.Support.SingleOrDefault(x => x.SeoTitle.Equals(seoTitle));
        }
    }
}
