using flower.Entities;
using flower.Helpers;
using flower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Orders
{
    public interface IOrderRepository
    {
        ValueTask<int> Create(int userId, CreateOrderRequest createOrderRequest);
        ValueTask<object> GetAll(DateTime? start_date, DateTime? end_date, float min_price, float max_price, StatusEnum status, string phone_number, string address,
            bool? paid, bool sort, string sortField, int limit, int offset, int user_id = 0);
        ValueTask<Order> GetDetail(int id, int user_id);
        ValueTask CancelOrder(int id, int user_id);
        ValueTask Update(int id, UpdateOrderRequest updateOrderRequest);
    }
}
