using WaterlilyProductManagement.Models.Reports;

namespace WaterlilyProductManagement.ViewModels.Reports;

public class ReportViewModel
{
    public IReadOnlyList<CategoryAveragePrice> CategoryAverages { get; set; }
        = Array.Empty<CategoryAveragePrice>();

    public HighestStockValueCategory? HighestStockValueCategory { get; set; }
}
