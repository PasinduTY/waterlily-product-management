
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaterlilyProductManagement.Data;
using WaterlilyProductManagement.Models;
using WaterlilyProductManagement.Repositories;

namespace WaterlilyProductManagement.Tests.Repositories;

[TestClass]
public class ReportRepositoryIntegrationTests
{
    [TestMethod]
    public async Task StoredProcedures_ReturnExpectedResults()
    {
        // Read the connection string from the main application's settings.
        var configPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../WaterlilyProductManagement/appsettings.json"));

        using var json = JsonDocument.Parse(
            await File.ReadAllTextAsync(configPath));

        var connectionString = json.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(connectionString));

        // Connect to the actual SQL Server database.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);

        // Keep all test data inside a transaction.
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        try
        {
            var categoryA = $"__Integration_A_{Guid.NewGuid():N}";
            var categoryB = $"__Integration_B_{Guid.NewGuid():N}";

            // Arrange: insert temporary products.
            context.Products.AddRange(
                new Product
                {
                    Name = "Test Product A1",
                    Category = categoryA,
                    Price = 100m,
                    Stock = 2
                },
                new Product
                {
                    Name = "Test Product A2",
                    Category = categoryA,
                    Price = 300m,
                    Stock = 3
                },
                new Product
                {
                    Name = "Test Product B1",
                    Category = categoryB,
                    Price = 500m,
                    Stock = 10
                },
                new Product
                {
                    Name = "Test Product B2",
                    Category = categoryB,
                    Price = 700m,
                    Stock = 10
                });

            await context.SaveChangesAsync();

            var repository = new ReportRepository(context);

            // Act: execute both stored procedures.
            var averages =
                await repository.GetAveragePriceByCategoryAsync();

            var highestStockValue =
                await repository.GetHighestStockValueCategoryAsync();

            // Assert: verify average price mapping and calculation.
            var averageA = averages.Single(
                x => x.Category == categoryA);

            var averageB = averages.Single(
                x => x.Category == categoryB);

            Assert.AreEqual(200m, averageA.AveragePrice);
            Assert.AreEqual(600m, averageB.AveragePrice);

            // Assert: verify highest stock value mapping.
            Assert.IsNotNull(highestStockValue);
            Assert.AreEqual(categoryB, highestStockValue.Category);
            Assert.AreEqual(12000m, highestStockValue.TotalStockValue);
        }
        finally
        {
            // Remove temporary data, even if an assertion fails.
            await transaction.RollbackAsync();
        }
    }
}

