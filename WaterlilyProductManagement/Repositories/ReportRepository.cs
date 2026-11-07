using Microsoft.EntityFrameworkCore;
using WaterlilyProductManagement.Data;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models.Reports;

namespace WaterlilyProductManagement.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryAveragePrice>>
        GetAveragePriceByCategoryAsync()
    {
        return await _context.Set<CategoryAveragePrice>()
            .FromSqlRaw("EXEC dbo.sp_GetAveragePriceByCategory")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<HighestStockValueCategory?>
        GetHighestStockValueCategoryAsync()
    {
        var results = await _context.Set<HighestStockValueCategory>()
            .FromSqlRaw("EXEC dbo.sp_GetHighestStockValueCategory")
            .AsNoTracking()
            .ToListAsync();

        return results.FirstOrDefault();
    }
}
