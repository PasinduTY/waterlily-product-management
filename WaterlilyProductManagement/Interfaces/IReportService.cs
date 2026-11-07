using WaterlilyProductManagement.Models.Reports;

namespace WaterlilyProductManagement.Interfaces;

public interface IReportService
{
    Task<IReadOnlyList<CategoryAveragePrice>> GetAveragePriceByCategoryAsync();

    Task<HighestStockValueCategory?> GetHighestStockValueCategoryAsync();
}
