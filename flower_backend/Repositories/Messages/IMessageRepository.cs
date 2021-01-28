using flower.Entities;
using flower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Messages
{
    public interface IMessageRepository
    {
        ValueTask<object> GetAll(string message, int limit, int offset);
        ValueTask<Message> FindById(int id);
        ValueTask<int> Create(CreateAndUpdateMessageRequest createAndUpdateMessageRequest);
        ValueTask Update(int id, CreateAndUpdateMessageRequest createAndUpdateMessageRequest);
        ValueTask Delete(int id);
    }
}
