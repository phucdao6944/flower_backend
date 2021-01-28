using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        public MessagesController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string message, int limit, int offset)
        {
            var messages = await _messageRepository.GetAll(message, limit, offset);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(int id)
        {
            var messages = await _messageRepository.FindById(id);
            return Ok(new ResponseForm<Message>(messages));
        }

        [Authorize]
        [HttpPost("admin")]
        public async Task<IActionResult> Create(CreateAndUpdateMessageRequest request)
        {
            int id = await _messageRepository.Create(request);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> Update(int id, CreateAndUpdateMessageRequest request)
        {
            await _messageRepository.Update(id, request);
            return Ok(new ResponseForm<object>(new { id = id }));
        }

        [Authorize]
        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _messageRepository.Delete(id);
            return Ok(new ResponseForm<object>(null));
        }
    }
}
