using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SafeBlock.io.Models
{
    public class Users : IUsers
    {
        private readonly SafeBlockContext _context = null;

        public Users(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.Where(x => x.Mail == "clint.mourlevat@gmail.com").ToList();
        }

        public User GetUserByMail(string mail)
        {
            return _context.Users.Where(x => x.Mail.Equals(mail.ToLower())).SingleOrDefault();
        }

        public bool IsUserByMail(string mail)
        {
            return _context.Users.Any(x => x.Mail.Equals(mail.ToLower()));
        }

        public void AddUser(User newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
