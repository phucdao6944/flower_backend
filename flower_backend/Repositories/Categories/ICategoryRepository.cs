using flower.Entities;
using flower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace flower.Repositories.Categories
{
    public interface ICategoryRepository
    {
        ValueTask<List<Category>> GetAll();
        ValueTask Delete(int id);
        ValueTask AddNew(CreateAndUpdateCategoryRequest request);
        ValueTask Update(int id, CreateAndUpdateCategoryRequest request);
        ValueTask<Category> FindByName(string name);
    }
}
