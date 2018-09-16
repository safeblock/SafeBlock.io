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
    }
}
