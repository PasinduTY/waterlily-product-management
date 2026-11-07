
using WaterlilyProductManagement.Models;

namespace WaterlilyProductManagement.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task AddAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(int id);
}

