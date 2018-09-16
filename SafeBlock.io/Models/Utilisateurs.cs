using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SafeBlock.io.Models
{
    public class Utilisateurs : IUtilisateurs
    {
        private readonly SafeBlockContext _context = null;

        public Utilisateurs(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<Users> GetUsers()
        {
            return _context.Users.Where(x => x.Mail == "clint.mourlevat@gmail.com").ToList();
        }
    }
}
