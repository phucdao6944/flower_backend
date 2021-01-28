using flower.Entities;
using flower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Users
{
    public interface IUserRepository
    {
        ValueTask<User> FindUserByUserNameAndPassWord(string username, string password);
        ValueTask<User> GetById(int id);
        ValueTask AddUser(AddNewUserRequest addNewUserRequest);
        ValueTask DeleteUser(int id);
        ValueTask UpdateUser(int id, UpdateUserRequest updateUserRequest);
        ValueTask<User> GetUserByUserName(string username);
    }
}
