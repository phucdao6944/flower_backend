using flower.Entities;
using flower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Bouquests
{
    public interface IBouquetRepository
    {
        ValueTask<object> GetAll(string name, int[] categoryIds, float minPrice, float maxPrice, bool sort, string sortField, int limit, int offset);
        ValueTask<int> Create(int userId, CreateBouquetRequest createBouquet);
        ValueTask<Bouquet> FindById(int id);
        ValueTask Update(int userId, int id, UpdateBouquetRequest updateBouquet);
        ValueTask Delete(int id);
    }
}
