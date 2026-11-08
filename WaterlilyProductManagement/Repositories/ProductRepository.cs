using Microsoft.EntityFrameworkCore;
using WaterlilyProductManagement.Data;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models;

namespace WaterlilyProductManagement.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.ProductId)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products
            .FindAsync(product.ProductId);

        if (existingProduct is null)
        {
            return false;
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }
}
