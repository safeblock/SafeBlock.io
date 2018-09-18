using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeBlock.io.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SafeBlock.io.Services;
using SafeBlock.io.Settings;
using VaultSharp;

namespace SafeBlock.io.Controllers
{
    public class BlogController : Controller
    {
        private SafeBlockContext _context;
        private IHostingEnvironment _env;
        private readonly IBlog _blog;

        public BlogController(IHostingEnvironment env, SafeBlockContext context, IBlog blog)
        {
            _context = context;
            _env = env;
            _blog = blog;
        }

        [Route("blog")]
        public IActionResult Index()
        {
            var blog = _blog.GetAllArticles();
            return View(blog);
        }

        [Route("blog/category/{category}")]
        public IActionResult Category(string category)
        {
            var blog = _blog.GetArticlesByCategory(category);
            return View(blog);
        }

        [Route("blog/{article}")]
        public IActionResult Article(string article)
        {
            var blogArticle = _blog.GetArticleBySeo(article);
            return View(blogArticle);
        }

        [Route(("blog/{tag}"))]
        public IActionResult Tag(string tag)
        {
            return View();
        }
    }
}