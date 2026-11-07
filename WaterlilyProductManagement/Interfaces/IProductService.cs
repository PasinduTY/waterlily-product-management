
using WaterlilyProductManagement.Models;

namespace WaterlilyProductManagement.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task AddAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(int id);
}

