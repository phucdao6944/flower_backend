using Dapper;
using flower.Entities;
using flower.Helpers;
using flower.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Users
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async ValueTask AddUser(AddNewUserRequest addNewUserRequest)
        {
            var now = DateTime.UtcNow;
            var userByUsername = await GetUserByUserName(addNewUserRequest.username);
            if (userByUsername != null)
            {
                throw new Exception("username already exists");
            }
            string[] fields = new string[] { "first_name", "last_name", "username", "password", "dob", "gender", "phone_number", "address", "created_at", "updated_at" };
            string sql = SqlForm.GenerateSqlString(fields, "users", true);
            var param = new
            {
                first_name = addNewUserRequest.first_name,
                last_name = addNewUserRequest.last_name,
                username = addNewUserRequest.username.Trim(),
                password = Security.HashPassword(addNewUserRequest.password),
                dob = addNewUserRequest.dob,
                gender = addNewUserRequest.gender,
                phone_number = addNewUserRequest.phone_number,
                address = addNewUserRequest.address,
                created_at = now,
                updated_at = now
            };
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>(sql, param);
                return query;
            });

        }
        
        public async ValueTask UpdateUser(int id, UpdateUserRequest updateUserRequest)
        {
            var now = DateTime.UtcNow;
            List<string> fields = new List<string>();
            fields.Add("updated_at");
            if (!string.IsNullOrWhiteSpace(updateUserRequest.first_name)) fields.Add("first_name");
            if (!string.IsNullOrWhiteSpace(updateUserRequest.last_name)) fields.Add("last_name");
            if (!string.IsNullOrWhiteSpace(updateUserRequest.email)) fields.Add("email");
            if (!string.IsNullOrWhiteSpace(updateUserRequest.address)) fields.Add("address");
            if (!string.IsNullOrWhiteSpace(updateUserRequest.phone_number)) fields.Add("phone_number");
            if (updateUserRequest.dob != null) fields.Add("dob");
            if (updateUserRequest.gender != null) fields.Add("gender");
            string sql = SqlForm.GenerateSqlString(fields.ToArray(), "users", false);
            sql += " and id = @id";
            var param = new
            {
                first_name = updateUserRequest.first_name,
                last_name = updateUserRequest.last_name,
                email = updateUserRequest.email,
                address = updateUserRequest.address,
                phone_number = updateUserRequest.phone_number,
                dob = updateUserRequest.dob,
                gender = updateUserRequest.gender,
                updated_at = now,
                id = id
            };
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>(sql, param);
                return query;
            });
        }

        public async ValueTask DeleteUser(int id)
        {
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>("DELETE FROM users where id = @id", new { id = id });
                return query;
            });
        }    

        public async ValueTask<User> FindUserByUserNameAndPassWord(string username, string password)
        {
            string passwordHashed = Security.HashPassword(password);
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM users where username = @username and password = @password", new { username = username.Trim(), password = passwordHashed });
                return query;
            });
        }

        public async ValueTask<User> GetById(int id)
        {
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM users where id = @id", new { id = id});
                return query;
            });
        }

        public async ValueTask<User> GetUserByUserName(string username)
        {
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM users where username = @username", new { username = username.Trim() });
                return query;
            });
        }
    }
}
