
using Microsoft.Extensions.Caching.Memory;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models;
using WaterlilyProductManagement.Constants;

namespace WaterlilyProductManagement.Services;

public delegate void ProductCallback(Product product);

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMemoryCache _cache;

    private const string AllProductsCacheKey = "Products_All";

    public event ProductCallback? ProductChanged;

    public ProductService(
        IProductRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    // Generic cache: keeps the result until it is removed.
    private async Task<T> CachedLong<T>(
        string key,
        Func<Task<T>> getData)
    {
        if (_cache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue!;
        }

        T result = await getData();

        _cache.Set(key, result);

        return result;
    }

    private async Task<T> Cached<T>(
        string key,
        Func<Task<T>> getData)
    {
        if (_cache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue!;
        }

        T result = await getData();

        _cache.Set(key, result, TimeSpan.FromMinutes(5));

        return result;
    }

    private void InvalidateProductAndReportCaches(int? productId = null)
    {
        _cache.Remove(CacheKeys.AllProducts);

        if (productId.HasValue)
        {
            _cache.Remove(CacheKeys.ProductById(productId.Value));
        }

        _cache.Remove(CacheKeys.AveragePriceByCategory);
        _cache.Remove(CacheKeys.HighestStockValueCategory);
    }


    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        return await CachedLong(
            CacheKeys.AllProducts,
            () => _repository.GetAllAsync());
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await Cached(
            CacheKeys.ProductById(id),
            () => _repository.GetByIdAsync(id));
    }

    public async Task AddAsync(Product product)
    {
        await _repository.AddAsync(product);

        InvalidateProductAndReportCaches(product.ProductId);

        ProductChanged?.Invoke(product);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        bool updated = await _repository.UpdateAsync(product);

        if (!updated)
            return false;

        InvalidateProductAndReportCaches(product.ProductId);

        ProductChanged?.Invoke(product);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Product? product = await _repository.GetByIdAsync(id);

        if (product is null)
            return false;

        bool deleted = await _repository.DeleteAsync(id);

        if (!deleted)
            return false;

        InvalidateProductAndReportCaches(id);

        ProductChanged?.Invoke(product);

        return true;
    }
}

