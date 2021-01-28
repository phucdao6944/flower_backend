using Dapper;
using flower.Entities;
using flower.Helpers;
using flower.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Categories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly IConfiguration _configuration;
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        public async ValueTask AddNew(CreateAndUpdateCategoryRequest request)
        {
            var now = DateTime.UtcNow;
            var findByName = await this.FindByName(request.name);
            if (findByName != null)
                throw new Exception("name already exists");
            string[] fields = new string[] { "name", "created_at", "updated_at" };
            string sql = SqlForm.GenerateSqlString(fields, "categories", true);
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync(sql, new { name = request.name, created_at = now, updated_at = now });
                return query;
            });
        }

        public async ValueTask Delete(int id)
        {
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Category>("DELETE FROM categories where id = @id", new { id = id });
                return query;
            });
        }

        public async ValueTask<Category> FindByName(string name)
        {
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Category>("SELECT * FROM categories where name = @name", new { name = name });
                return query;
            });
        }

        public async ValueTask<List<Category>> GetAll()
        {
            return (List<Category>)await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<Category>("SELECT * FROM categories", null);
                return query;
            });
        }

        public async ValueTask Update(int id, CreateAndUpdateCategoryRequest request)
        {
            var now = DateTime.UtcNow;
            var findByName = await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Category>("SELECT * FROM categories where name = @name and id != @id", new { name = request.name, id = id });
                return query;
            });
            if (findByName != null)
                throw new Exception("name already exists");
            string[] fields = new string[] { "name", "updated_at" };
            string sql = SqlForm.GenerateSqlString(fields, "categories", false);
            sql += " and id = @id";
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync(sql, new { name = request.name, updated_at = now, id = id });
                return query;
            });
        }
    }
}
