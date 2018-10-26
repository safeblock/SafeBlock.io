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
        private readonly SafeBlockContext _context;

        public Users(DbContextOptions options)
        {
            _context = new SafeBlockContext(options);
        }

        public List<User> GetAllUsers()
        {
            return null;
            //return _context.Users.ToList();
        }

        public User GetUserByMail(string mail)
        {
            return null;
            //return _context.Users.SingleOrDefault(x => x.Mail.Equals(mail.ToLower()));
        }

        public bool IsUserByMail(string mail)
        {
            return false;
            //return _context.Users.Any(x => x.Mail.Equals(mail.ToLower()));
        }

        public void AddUser(User newUser)
        {
            /*_context.Users.Add(newUser);
            _context.SaveChanges();*/
        }
    }
}
