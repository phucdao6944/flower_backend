using Dapper;
using flower.Entities;
using flower.Helpers;
using flower.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace flower.Repositories.Messages
{
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        private readonly IConfiguration _configuration;
        public MessageRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async ValueTask<int> Create(CreateAndUpdateMessageRequest createAndUpdateMessageRequest)
        {
            var now = DateTime.UtcNow;
            string[] fields = new string[] { "messages", "created_at", "updated_at"};
            string sql = SqlForm.GenerateSqlString(fields, "messages", true);
            var param = new
            {
                messages = createAndUpdateMessageRequest.message,
                created_at = now,
                updated_at = now,
            };
            var id = await WithConnection(async conn =>
            {
                int query = await conn.QuerySingleAsync<int>(sql, param);
                return query;
            });
            return id;
        }

        public async ValueTask Delete(int id)
        {
            await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync("DELETE FROM messages where id = @id", new { id = id });
                return query;
            });
        }

        public async ValueTask<Message> FindById(int id)
        {
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Message>("SELECT * FROM messages where id = @id", new { id = id });
                return query;
            });
        }

        public async ValueTask<object> GetAll(string message, int limit, int offset)
        {
            limit = limit == 0 ? 10 : limit;
            string sql = "SELECT * FROM messages WHERE 1 = 1";
            string sqlCount = "SELECT COUNT(*) as total FROM messages WHERE 1 = 1";
            string condition = "";
            var param = new
            {
                messages = "%" + message + "%",
                limit = limit,
                offset = offset
            };
            if (!string.IsNullOrWhiteSpace(message))
                condition += " and messages LIKE @messages";
            var totalResult = await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<dynamic>(sqlCount + condition, param);
                return query;
            });
            condition += " ORDER BY created_at DESC";
            // Pagination
            condition += " OFFSET @offset ROWS " +
                " FETCH NEXT @limit ROWS ONLY";
            var messages = await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<Message>(sql + condition, param);
                return query;
            });
            var pagination = new Pagination(limit == 0 ? 10 : limit, offset, totalResult.total);
            return new ResponseListForm<Message>(messages, pagination);
        }

        public async ValueTask Update(int id, CreateAndUpdateMessageRequest createAndUpdateMessageRequest)
        {
            var now = DateTime.UtcNow;
            string[] fields = new string[] { "messages", "updated_at" };
            string sql = SqlForm.GenerateSqlString(fields, "messages", false);
            sql += " and id = @id";
            var param = new
            {
                messages = createAndUpdateMessageRequest.message,
                updated_at = now,
                id = id
            };
            await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync(sql, param);
                return query;
            });
        }
    }
}
