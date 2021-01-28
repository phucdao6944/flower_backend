using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Bouquests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Controllers
{
    [Route("api/bouquet")]
    [ApiController]
    public class BouquetsController : ControllerBase
    {
        private readonly IBouquetRepository _bouquetRepository;
        public BouquetsController(IBouquetRepository bouquetRepository)
        {
            _bouquetRepository = bouquetRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string name,[CheckIds("categoryIds")] string categoryIds, float minPrice, float maxPrice, bool sort, string sortField, int limit, int offset)
        {
            int[] ids = new int[] { };
            if (!string.IsNullOrWhiteSpace(categoryIds))
                ids = Array.ConvertAll(categoryIds.Split(','), p => int.Parse(p.Trim()));
            var bouquests = await _bouquetRepository.GetAll(name, ids, minPrice, maxPrice, sort, sortField, limit, offset);
            return Ok(bouquests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(int id)
        {
            var bouquet = await _bouquetRepository.FindById(id);
            return Ok(new ResponseForm<Bouquet>(bouquet));
        }

        [Authorize]
        [HttpPost("admin")]
        public async Task<IActionResult> Create([FromBody] CreateBouquetRequest createBouquet)
        {
            HttpContext context = Request.HttpContext;
            var userLogin = (User)context.Items["User"];
            int id = await _bouquetRepository.Create(userLogin.id, createBouquet);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBouquetRequest updateBouquet)
        {
            HttpContext context = Request.HttpContext;
            var userLogin = (User)context.Items["User"];
            await _bouquetRepository.Update(userLogin.id, id, updateBouquet);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bouquetRepository.Delete(id);
            return Ok(new ResponseForm<object>(null));
        }
    }
}
