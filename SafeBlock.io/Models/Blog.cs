using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SafeBlock.io.Models
{
    public class Blogs : IBlogs
    {
        private readonly SafeBlockContext _context = null;

        public Blogs(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<Article> GetAllArticles()
        {
            return _context.Blogs.ToList();
        }
    }
}
