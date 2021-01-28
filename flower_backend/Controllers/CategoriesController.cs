using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAll();
            return Ok(new ResponseListForm<object>(categories));
        }


        [Authorize]
        [HttpPost("admin")]
        public async Task<IActionResult> Post([FromBody] CreateAndUpdateCategoryRequest request)
        {
            try
            {
                await _categoryRepository.AddNew(request);
                return Ok(new ResponseForm<object>(null));
            }
            catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }

        }

        [Authorize]
        [HttpPut("admin")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateAndUpdateCategoryRequest request)
        {
            try
            {
                await _categoryRepository.Update(id, request);
                return Ok(new ResponseForm<object>(null));
            }
            catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }
        }

        [Authorize]
        [HttpDelete("admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryRepository.Delete(id);
                return Ok(new ResponseForm<object>(null));
            }
            catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }
        }
    }
}
