using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeBlock.io.Models
{
    public interface IUsers
    {
        List<User> GetAllUsers();
        User GetUserByMail(string mail);
        bool IsUserByMail(string mail);
        void AddUser(User newUser);
    }
}
