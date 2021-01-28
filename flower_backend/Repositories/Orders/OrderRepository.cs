using Dapper;
using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Bouquests;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace flower.Repositories.Orders
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IBouquetRepository _bouquetRepository;
        public OrderRepository(IConfiguration configuration, IBouquetRepository bouquetRepository) : base(configuration)
        {
            _configuration = configuration;
            _bouquetRepository = bouquetRepository;
        }
        public async ValueTask<int> Create(int userId, CreateOrderRequest createOrderRequest)
        {
            var now = DateTime.UtcNow;
            now = DateTime.Parse("2021-01-27T20:45:00.000");
            var startTime = int.Parse(_configuration.GetSection("WorkingTime:Start").Value);
            var endTime = int.Parse(_configuration.GetSection("WorkingTime:End").Value);
            var shippingTimes = int.Parse(_configuration.GetSection("ShippingTimes").Value);
            //Get bouquets
            var bouquetIds = createOrderRequest.order_details.Select(x => x.bouquet_id).ToList();
            var bouquets = await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<Bouquet>("SELECT * FROM bouquets where id in @id", new { id = bouquetIds });
                return query;
            });
            double totalPrice = 0;
            foreach(Bouquet bouquet in bouquets)
            {
                foreach (AddNewOrderDetailRequest detailRequest in createOrderRequest.order_details)
                {
                    if(bouquet.id == detailRequest.bouquet_id)
                    {
                        totalPrice += bouquet.price * detailRequest.quantity;
                    }    
                }    
            }    

            // Insert order table
            DateTime expectedTime;
            int currentHour = int.Parse(now.ToString("HH"));
            if((currentHour < endTime && currentHour >= startTime) || (currentHour >= startTime && currentHour - endTime >= -shippingTimes))
            {
                var compareDate = DateTime.UtcNow.Date.AddHours(endTime) - now;
                //if (compareDate.TotalSeconds > 0)
                //{
                //    expectedTime = DateTime.UtcNow.Date.Add(new TimeSpan(0, 0, 0)).AddDays(1).AddHours(startTime).AddSeconds(shippingTimes * 3600 - compareDate.TotalSeconds);
                //    throw new Exception(expectedTime.ToString());
                //}
                //else 
                if (compareDate.TotalSeconds < 0 && compareDate.TotalSeconds > 0)
                {
                    expectedTime = DateTime.UtcNow.Date.Add(new TimeSpan(0, 0, 0)).AddDays(1).AddSeconds((startTime + shippingTimes) * 3600);
                }
                else
                {
                    expectedTime = DateTime.UtcNow.Date.Add(new TimeSpan(0, 0, 0)).AddHours(endTime);
                }
            } 
            else
            {
                expectedTime = DateTime.UtcNow.Date.AddHours(shippingTimes + startTime);
            }                
            var fields = new string[] { "user_id", "address", "phone_number", "status", "expected_time", "created_at", "total_price", "paid" };
            var sql = SqlForm.GenerateSqlString(fields, "orders", true);
            var param = new
            {
                user_id = userId,
                address = createOrderRequest.address,
                phone_number = createOrderRequest.phone_number,
                status = StatusEnum.New,
                expected_time = expectedTime,
                paid = false,
                created_at = now,
                total_price = totalPrice
            };
            var id = await WithConnection(async conn =>
            {
                int query = await conn.QuerySingleAsync<int>(sql, param);
                return query;
            });
            foreach (Bouquet bouquet in bouquets)
            {
                foreach (AddNewOrderDetailRequest detailRequest in createOrderRequest.order_details)
                {
                    if (bouquet.id == detailRequest.bouquet_id)
                    {
                        var fieldsDetail = new string[] { "order_id", "bouquet_id", "quantity", "total_price", "message" };
                        var sqlDetail = SqlForm.GenerateSqlString(fieldsDetail, "order_details", true);
                        var paramDetail = new
                        {
                            order_id = id,
                            bouquet_id = bouquet.id,
                            quantity = detailRequest.quantity,
                            total_price = bouquet.price * detailRequest.quantity,
                            message = detailRequest.message
                        };
                        await WithConnection(async conn =>
                        {
                            var query = await conn.QuerySingleAsync(sqlDetail, paramDetail);
                            return query;
                        });
                    }
                }
            }
            return id;
        }

        public async ValueTask<object> GetAll(DateTime? start_date, DateTime? end_date, float min_price, float max_price, StatusEnum status, string phone_number, string address, 
            bool? paid, bool sort, string sort_field, int limit, int offset, int user_id = 0)
        {
            limit = limit == 0 ? 10 : limit;
            string sql = "SELECT * FROM orders as ord LEFT JOIN users as us on ord.user_id = us.id where 1 = 1 ";
            string sqlCount = "SELECT COUNT(*) as total FROM orders as ord LEFT JOIN users as us on ord.user_id = us.id where 1 = 1 ";
            var param = new
            {
                start_date = start_date,
                end_date = end_date,
                min_price = min_price,
                max_price = max_price,
                status = status,
                phone_number = "%" + phone_number + "%",
                address = "%" + address + "%",
                paid = paid,
                user_id = user_id,
                limit = limit,
                offset = offset
            };

            string condition = "";
            if (start_date != null && end_date == null)
                condition += " and ord.created_at >= @start_date";
            else if (start_date == null && end_date != null)
                condition += " and ord.created_at >= @end_date";
            else if (start_date == null && end_date != null)
                condition += " and ord.created_at BETWEEN @start_date AND @end_date";

            if (min_price > 0 && max_price <= 0)
                condition += " and ord.total_price >= @min_price";
            else if (max_price > 0 && min_price <= 0)
                condition += " and ord.total_price <= @max_price";
            else if (max_price > 0 && min_price > 0)
                condition += " and ord.total_price BETWEEN @min_price and @max_price";

            if (status > 0)
                condition += " and ord.status >= @status";
            if (!string.IsNullOrWhiteSpace(phone_number))
                condition += " and ord.phone_number LIKE @phone_number";
            if (!string.IsNullOrWhiteSpace(address))
                condition += " and ord.address LIKE @address";
            if (paid != null)
                condition += " and ord.paid = @paid";
            if (user_id > 0)
                condition += " and ord.user_id = @user_id";

            // Sum all results
            var totalResults = (dynamic)await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<dynamic>(sqlCount + condition, param);
                return query;
            });

            // Sort by field
            if (!string.IsNullOrEmpty(sort_field))
            {
                string[] whiteListSort =
                {
                        "paid", "created_at", "total_price"
                };
                if (Array.IndexOf(whiteListSort, sort_field) > 0)
                {
                    condition += " order by ord." + sort_field;
                    condition += sort ? " DESC" : " ASC";
                }
            }
            else
            {
                condition += " ORDER BY ord.created_at DESC";
            }

            // Pagination
            condition += " OFFSET @offset ROWS " +
                " FETCH NEXT @limit ROWS ONLY";
            //throw new Exception(condition);
            // Result Multi-Mapping
            var orders = (List<Order>)await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<Order, User, Order>(sql + condition, (order, user) =>
                {
                    order.user = user;
                    return order;
                },
                splitOn: "id", param: param);
                return query.Distinct().ToList();
            });
            var pagination = new Pagination(limit == 0 ? 10 : limit, offset, totalResults.total);
            return new ResponseListForm<Order>(orders, pagination);
        }

        public async ValueTask<Order> GetDetail(int id, int user_id = 0)
        {
            string sql = "SELECT * FROM orders as ord LEFT JOIN users as us on ord.user_id = us.id where ord.id = @id ";
            if(user_id > 0)
            {
                sql += " and ord.user_id = @user_id";
            }
            var param = new
            {
                id = id,
                user_id = user_id
            };
            var orders = await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<Order, User, Order>(sql, (order, user) =>
                {
                    order.user = user;
                    return order;
                },
                splitOn: "id", param: param);
                return query.Distinct().ToList();
            });
            if(orders != null)
            {
                var order = orders[0];
                string sqlDetail = "SELECT * FROM order_details as ord INNER JOIN bouquets as bou on bou.id = ord.bouquet_id LEFT JOIN bouquet_images as img on bou.id = img.bouquet_id where" +
                    " ord.order_id = @id";
                var bouquests = (List<OrderDetail>)await WithConnection(async conn =>
                {
                    var bouquestDictionary = new Dictionary<int, OrderDetail>();
                    var query = await conn.QueryAsync<OrderDetail, BouquetImage, OrderDetail>(sqlDetail, (bouquest, bouquestImage) =>
                    {
                        OrderDetail bouquetEntry;
                        if (!bouquestDictionary.TryGetValue(bouquest.bouquet_id, out bouquetEntry))
                        {
                            bouquetEntry = bouquest;
                            bouquetEntry.images = new List<BouquetImage>();
                            bouquestDictionary.Add(bouquetEntry.bouquet_id, bouquetEntry);
                        }
                        bouquetEntry.images.Add(bouquestImage);
                        return bouquetEntry;
                    },
                    splitOn: "id", param: new { id = id });
                    return query.Distinct().ToList();
                });
                order.orderDetails = bouquests;
                return order;
            }
            return null;
        }

        public async ValueTask CancelOrder(int id, int user_id)
        {
            string sql = "SELECT * FROM orders where id = @id and user_id = @user_id";
            var order = await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Order>(sql, new { id = id, user_id = user_id });
                return query;
            });
            if (order == null)
                throw new Exception("Order not found");
            if (order.status != StatusEnum.New)
                throw new Exception("Cannot cancel this order");

            var fields = new string[] { "status" };
            var sqlCancel = SqlForm.GenerateSqlString(fields, "orders", false);
            sqlCancel += " and id = @id";
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<Order>(sqlCancel, new { id = id, status = StatusEnum.Cancelled });
                return query;
            });
        }

        public async ValueTask Update(int id, UpdateOrderRequest updateOrderRequest)
        {
            var now = DateTime.UtcNow;
            List<string> fields = new List<string>();
            if (!string.IsNullOrWhiteSpace(updateOrderRequest.address))
                fields.Add("address");
            if (!string.IsNullOrWhiteSpace(updateOrderRequest.phone_number))
                fields.Add("phone_number");
            if (updateOrderRequest.status > 0)
                fields.Add("status");
            string sql = SqlForm.GenerateSqlString(fields.ToArray(), "orders", false);
            sql += " and id = @id";
            var param = new
            {
                id = id,
                address = updateOrderRequest.address,
                phone_number = updateOrderRequest.phone_number,
                status = updateOrderRequest.status
            };
            await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync(sql, param);
                return query;
            });
        }
    }
}
