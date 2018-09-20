using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SafeBlock.io.Models
{
    public class Blog : IBlog
    {
        private readonly SafeBlockContext _context = null;

        public Blog(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<Article> GetAllArticles()
        {
            return _context.Blog.ToList();
        }

        public List<Article> GetArticlesByCategory(string category)
        {
            return _context.Blog.Where(x => x.Category.Equals(category)).ToList();
        }

        public List<Article> GetArticlesByTag(string tag)
        {
            return _context.Blog.Where(x => x.Tags.Contains(tag)).ToList();
        }

        public Article GetArticleBySeo(string seoTitle)
        {

            return _context.Blog.Where(x => x.SeoTitle.Equals(seoTitle)).SingleOrDefault();
        }

        public int GetArticleNumberByCategory(string category)
        {
            return _context.Blog.Where(x => x.Category.Equals(category)).ToList().Count;
        }

        public List<string> GetAllTags()
        {
            return _context.Blog.Select(x => x.Tags).ToList();
        }
    }
}
