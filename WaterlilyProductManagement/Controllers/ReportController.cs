using Microsoft.AspNetCore.Mvc;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.ViewModels.Reports;

namespace WaterlilyProductManagement.Controllers;

public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var categoryAverages =
            await _reportService.GetAveragePriceByCategoryAsync();

        var highestStockValueCategory =
            await _reportService.GetHighestStockValueCategoryAsync();

        var viewModel = new ReportViewModel
        {
            CategoryAverages = categoryAverages,
            HighestStockValueCategory = highestStockValueCategory
        };

        return View(viewModel);
    }
}
