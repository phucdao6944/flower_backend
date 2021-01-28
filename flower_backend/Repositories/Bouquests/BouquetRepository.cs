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

namespace flower.Repositories.Bouquests
{
    public class BouquetRepository : BaseRepository, IBouquetRepository
    {
        private readonly IConfiguration _configuration;
        public BouquetRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async ValueTask<int> Create(int userId, CreateBouquetRequest createBouquetRequest)
        {
            var now = DateTime.UtcNow;
            // Check name already exists
            var findByName = await this.FindByName(createBouquetRequest.name);
            if (findByName != null)
                throw new Exception("Name already exists");

            // Save bouquet to database
            string[] fields = new string[] { "name", "description", "category_id", "price", "user_create", "created_at", "updated_at" };
            string sql = SqlForm.GenerateSqlString(fields, "bouquets", true);
            var param = new
            {
                name = createBouquetRequest.name,
                description = createBouquetRequest.description,
                category_id = createBouquetRequest.category_id,
                price = createBouquetRequest.price,
                user_create = userId,
                created_at = now,
                updated_at = now
            };
            var id = await WithConnection(async conn =>
            {
                int query = await conn.QuerySingleAsync<int>(sql, param);
                return query;
            });

            // Upload images and save images to database
            foreach(string image in createBouquetRequest.images)
            {
                string fileName = ImageHelper.SaveBase64(image);
                var fieldsImg = new string[] { "image_url", "bouquet_id", "created_at", "updated_at" };
                var sqlImg = SqlForm.GenerateSqlString(fieldsImg, "bouquet_images", true);
                var paramImg = new
                {
                    image_url = fileName,
                    bouquet_id = id,
                    created_at = now,
                    updated_at = now
                };
                await WithConnection(async conn =>
                {
                    int query = await conn.QuerySingleAsync<int>(sqlImg, paramImg);
                    return query;
                });
            }    
            return id;
        }

        public async ValueTask<Bouquet> FindById(int id)
        {
            string sql = "SELECT cat.name as category_name, us.id as user_id, us.username, bou.*, img.* " +
                "FROM bouquets as bou " +
                "LEFT JOIN bouquet_images as img ON bou.id = img.bouquet_id " +
                "INNER JOIN categories as cat ON cat.id = bou.category_id " +
                "INNER JOIN users as us ON us.id = bou.user_create where id = @id";
            var bouquests = (List<Bouquet>)await WithConnection(async conn =>
            {
                var bouquestDictionary = new Dictionary<int, Bouquet>();
                var query = await conn.QueryAsync<Bouquet, BouquetImage, Bouquet>(sql, (bouquest, bouquestImage) =>
                {
                    Bouquet bouquetEntry;
                    if (!bouquestDictionary.TryGetValue(bouquest.id, out bouquetEntry))
                    {
                        bouquetEntry = bouquest;
                        bouquetEntry.images = new List<BouquetImage>();
                        bouquestDictionary.Add(bouquetEntry.id, bouquetEntry);
                    }
                    bouquetEntry.images.Add(bouquestImage);
                    return bouquetEntry;
                },
                splitOn: "id", param: new { id = id });
                return query.Distinct().ToList();
            });
            if (bouquests.Count > 0)
                return bouquests[0];
            else
                return null;
        }

        public async ValueTask<Bouquet> FindByName(string name)
        {
            string sql = "SELECT cat.name as category_name, us.id as user_id, us.username, bou.*, img.* " +
                "FROM bouquets as bou " +
                "LEFT JOIN bouquet_images as img ON bou.id = img.bouquet_id " +
                "INNER JOIN categories as cat ON cat.id = bou.category_id " +
                "INNER JOIN users as us ON us.id = bou.user_create where bou.name = @name ";
            var bouquests = (List<Bouquet>)await WithConnection(async conn =>
            {
                var bouquestDictionary = new Dictionary<int, Bouquet>();
                var query = await conn.QueryAsync<Bouquet, BouquetImage, Bouquet>(sql, (bouquest, bouquestImage) =>
                {
                    Bouquet bouquetEntry;
                    if (!bouquestDictionary.TryGetValue(bouquest.id, out bouquetEntry))
                    {
                        bouquetEntry = bouquest;
                        bouquetEntry.images = new List<BouquetImage>();
                        bouquestDictionary.Add(bouquetEntry.id, bouquetEntry);
                    }
                    bouquetEntry.images.Add(bouquestImage);
                    return bouquetEntry;
                },
                splitOn: "id", param: new { name = name });
                return query.Distinct().ToList();
            });
            if (bouquests.Count > 0)
                return bouquests[0];
            else
                return null;
        }

        public async ValueTask<object> GetAll(string name, int[] categoryIds, float minPrice, float maxPrice, bool sort, string sortField, int limit, int offset)
        {
            string sql = "SELECT cat.name as category_name, us.id as user_id, us.username, bou.*, img.* " +
                "FROM bouquets as bou " +
                "LEFT JOIN bouquet_images as img ON bou.id = img.bouquet_id " +
                "INNER JOIN categories as cat ON cat.id = bou.category_id " +
                "INNER JOIN users as us ON us.id = bou.user_create where 1 = 1 ";
            string sqlCount = "Select count(*) as total FROM bouquets as bou INNER JOIN categories as cat ON cat.id = bou.category_id INNER JOIN users as us ON us.id = bou.user_create where 1 = 1 ";
            var param = new
            {
                name = "%" + name + "%",
                categoryIds = categoryIds,
                maxPrice = maxPrice,
                minPrice = minPrice,
                limit = limit == 0 ? 10 : limit,
                offset = offset
            };

            // Condition select statement sql
            string condition = "";
            if (!string.IsNullOrWhiteSpace(name))
                condition += " and bou.name LIKE @name ";
            if(categoryIds.Length > 0)
                condition += " and bou.category_id IN @categoryIds ";
            if(minPrice > 0 && maxPrice > 0)
                condition += " and bou.price BETWEEN @minPrice and @maxPrice ";
            else if(maxPrice > 0 && minPrice <= 0)
                condition += " and bou.price <= @maxPrice ";
            else if (maxPrice <= 0 && minPrice > 0)
                condition += " and bou.price >= @minPrice ";

            // Sum all results
            var totalResults = (dynamic)await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<dynamic>(sqlCount + condition, param);
                return query;
            });

            // Sort by field
            if (!string.IsNullOrEmpty(sortField))
            {
                string[] whiteListSort =
                {
                        "name", "created_at", "id", "price", "user_create"
                };
                if (Array.IndexOf(whiteListSort, sortField) > 0)
                {
                    condition += " order by bou." + sortField;
                    condition += sort ? " DESC" : " ASC";
                }
            }
            else
            {
                condition += " ORDER BY bou.created_at DESC";
            }

            // Pagination
            condition += " OFFSET @offset ROWS " +
                " FETCH NEXT @limit ROWS ONLY";

            // Result Multi-Mapping
            var bouquests = (List<Bouquet>)await WithConnection(async conn =>
            {
                var bouquestDictionary = new Dictionary<int, Bouquet>();
                var query = await conn.QueryAsync<Bouquet, BouquetImage, Bouquet>(sql + condition, (bouquest, bouquestImage) =>
                {
                    Bouquet bouquetEntry;
                    if (!bouquestDictionary.TryGetValue(bouquest.id, out bouquetEntry))
                    {
                        bouquetEntry = bouquest;
                        bouquetEntry.images = new List<BouquetImage>();
                        bouquestDictionary.Add(bouquetEntry.id, bouquetEntry);
                    }
                    bouquetEntry.images.Add(bouquestImage);
                    return bouquetEntry;
                },
                splitOn: "id", param: param);
                return query.Distinct().ToList();
            });
            var pagination = new Pagination(limit == 0 ? 10 : limit, offset, totalResults.total);
            return new ResponseListForm<Bouquet>(bouquests, pagination);
        }

        public async ValueTask Update(int userId, int id, UpdateBouquetRequest updateBouquet)
        {
            var now = DateTime.UtcNow;
            List<string> fields = new List<string>();
            fields.Add("updated_at");
            fields.Add("user_create");
            if (updateBouquet.category_id > 0) fields.Add("category_id");
            if (!string.IsNullOrWhiteSpace(updateBouquet.description)) fields.Add("description");
            if (!string.IsNullOrWhiteSpace(updateBouquet.name)) fields.Add("name");
            if (updateBouquet.price > 0) fields.Add("price");
            string sql = SqlForm.GenerateSqlString(fields.ToArray(), "bouquets", false);
            sql += " and id = @id";
            var param = new
            {
                id = id,
                updated_at = now,
                user_create = userId,
                category_id = updateBouquet.category_id,
                description = updateBouquet.description,
                name = updateBouquet.name,
                price = updateBouquet.price,
            };
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<User>(sql, param);
                return query;
            });

            // Delete old images and add new images
            List<int> imageIds = new List<int>();
            if(updateBouquet.images != null)
            {
                foreach (string image in updateBouquet.images)
                {
                    if (Regex.IsMatch(image, @"^\d+$"))
                    {
                        imageIds.Add(int.Parse(image));
                    }
                    else
                    {
                        string fileName = ImageHelper.SaveBase64(image);
                        var fieldsImg = new string[] { "image_url", "bouquet_id", "created_at", "updated_at" };
                        var sqlImg = SqlForm.GenerateSqlString(fieldsImg, "bouquet_images", true);
                        var paramImg = new
                        {
                            image_url = fileName,
                            bouquet_id = id,
                            created_at = now,
                            updated_at = now
                        };
                        var idNewImg = await WithConnection(async conn =>
                        {
                            int query = await conn.QueryFirstOrDefaultAsync<int>(sqlImg, paramImg);
                            return query;
                        });
                        imageIds.Add(idNewImg);
                    }
                }
                if (imageIds.Count > 0)
                {
                    var images = await WithConnection(async conn =>
                    {
                        var query = await conn.QueryAsync<BouquetImage>("SELECT * FROM bouquet_images where bouquet_id = @bouquet_id and id NOT IN @id ", new { bouquet_id = id, id = imageIds.ToArray() });
                        return query;
                    });
                    if(images != null)
                    {
                        foreach (BouquetImage bouquetImage in images)
                        {
                            ImageHelper.DeleteImage(bouquetImage.image_url);
                        }
                    }
                    await WithConnection(async conn =>
                    {
                        var query = await conn.QueryFirstOrDefaultAsync("DELETE FROM bouquet_images where bouquet_id = @bouquet_id and id NOT IN @id ", new { bouquet_id = id, id = imageIds.ToArray() });
                        return query;
                    });
                }
            }    
        }

        public async ValueTask Delete(int id)
        {
            var images = await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<BouquetImage>("SELECT * FROM bouquet_images where bouquet_id = @bouquet_id", new { bouquet_id = id});
                return query;
            });
            if (images != null)
            {
                foreach (BouquetImage bouquetImage in images)
                {
                    ImageHelper.DeleteImage(bouquetImage.image_url);
                }
            }
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync("DELETE FROM bouquet_images where bouquet_id = @bouquet_id", new { bouquet_id = id});
                return query;
            });
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync("DELETE FROM bouquets where id = @id", new { id = id });
                return query;
            });
        }
    }
}
