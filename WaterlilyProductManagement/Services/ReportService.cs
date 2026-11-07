using Microsoft.Extensions.Caching.Memory;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models.Reports;
using WaterlilyProductManagement.Constants;

namespace WaterlilyProductManagement.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;
    private readonly IMemoryCache _cache;

    public ReportService(
        IReportRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

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

    public async Task<IReadOnlyList<CategoryAveragePrice>>
    GetAveragePriceByCategoryAsync()
    {
        return await CachedLong(
            CacheKeys.AveragePriceByCategory,
            () => _repository.GetAveragePriceByCategoryAsync());
    }

    public async Task<HighestStockValueCategory?>
        GetHighestStockValueCategoryAsync()
    {
        return await Cached(
            CacheKeys.HighestStockValueCategory,
            () => _repository.GetHighestStockValueCategoryAsync());
    }
}
