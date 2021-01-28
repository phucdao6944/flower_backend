using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> New([FromBody] CreateOrderRequest createOrderRequest)
        {
            HttpContext context = Request.HttpContext;
            var userLogin = (User)context.Items["User"];
            var id = await _orderRepository.Create(userLogin.id, createOrderRequest);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAll(DateTime? start_date, DateTime? end_date, float min_price, float max_price, StatusEnum status, string phone_number, string address,
            bool? paid, bool sort, string sortField, int limit, int offset)
        {
            var orders = await _orderRepository.GetAll(start_date, end_date, min_price, max_price, status, phone_number, address, paid, sort, sortField, limit, offset);
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var orders = await _orderRepository.GetDetail(id, 0);
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailByUser(int id)
        {
            HttpContext context = Request.HttpContext;
            var userLogin = (User)context.Items["User"];
            var order = await _orderRepository.GetDetail(id, userLogin.id);
            return Ok(new ResponseForm<Order>(order));
        }

        [Authorize]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderRequest updateOrderRequest)
        {
            await _orderRepository.Update(id, updateOrderRequest);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                HttpContext context = Request.HttpContext;
                var userLogin = (User)context.Items["User"];
                await _orderRepository.CancelOrder(id, userLogin.id);
                return Ok(new ResponseForm<object>(null));
            } catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }
        }
    }
}
