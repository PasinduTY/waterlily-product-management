
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models.Reports;
using WaterlilyProductManagement.Services;

namespace WaterlilyProductManagement.Tests.Services;

[TestClass]
public class ReportServiceTests
{
    private Mock<IReportRepository> _mockRepository = null!;
    private MemoryCache _cache = null!;
    private ReportService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IReportRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new ReportService(
            _mockRepository.Object,
            _cache);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _cache.Dispose();
    }

    [TestMethod]
    public async Task GetAveragePriceByCategoryAsync_WhenCalledTwice_UsesCachedResult()
    {
        // Arrange
        IReadOnlyList<CategoryAveragePrice> expectedResult =
        [
            new CategoryAveragePrice
            {
                Category = "Electronics",
                AveragePrice = 1500m
            },
            new CategoryAveragePrice
            {
                Category = "Furniture",
                AveragePrice = 2500m
            }
        ];

        _mockRepository
            .Setup(r => r.GetAveragePriceByCategoryAsync())
            .ReturnsAsync(expectedResult);

        // Act
        var firstResult =
            await _service.GetAveragePriceByCategoryAsync();

        var secondResult =
            await _service.GetAveragePriceByCategoryAsync();

        // Assert
        Assert.AreSame(expectedResult, firstResult);
        Assert.AreSame(firstResult, secondResult);

        _mockRepository.Verify(
            r => r.GetAveragePriceByCategoryAsync(),
            Times.Once);
    }

    [TestMethod]
    public async Task GetHighestStockValueCategoryAsync_WhenCalledTwice_UsesCachedResult()
    {
        // Arrange
        var expectedResult = new HighestStockValueCategory
        {
            Category = "Electronics",
            TotalStockValue = 250000m
        };

        _mockRepository
            .Setup(r => r.GetHighestStockValueCategoryAsync())
            .ReturnsAsync(expectedResult);

        // Act
        var firstResult =
            await _service.GetHighestStockValueCategoryAsync();

        var secondResult =
            await _service.GetHighestStockValueCategoryAsync();

        // Assert
        Assert.AreSame(expectedResult, firstResult);
        Assert.AreSame(firstResult, secondResult);

        _mockRepository.Verify(
            r => r.GetHighestStockValueCategoryAsync(),
            Times.Once);
    }
}

