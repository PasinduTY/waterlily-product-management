using WaterlilyProductManagement.Models.Reports;

namespace WaterlilyProductManagement.Interfaces;

public interface IReportRepository
{
    Task<IReadOnlyList<CategoryAveragePrice>> GetAveragePriceByCategoryAsync();

    Task<HighestStockValueCategory?> GetHighestStockValueCategoryAsync();
}
