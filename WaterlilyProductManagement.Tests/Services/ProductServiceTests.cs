
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WaterlilyProductManagement.Constants;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models;
using WaterlilyProductManagement.Models.Reports;
using WaterlilyProductManagement.Services;

namespace WaterlilyProductManagement.Tests.Services;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _mockRepository = null!;
    private IMemoryCache _cache = null!;
    private ProductService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();

        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new ProductService(_mockRepository.Object, _cache);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _cache.Dispose();
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        IReadOnlyList<Product> products = new List<Product>
        {
            new Product
            {
                ProductId = 1,
                Name = "Keyboard",
                Category = "Electronics",
                Price = 2500,
                Stock = 10
            },
            new Product
            {
                ProductId = 2,
                Name = "Office Chair",
                Category = "Furniture",
                Price = 15000,
                Stock = 5
            }
        };

        _mockRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.HasCount(2, result);
        Assert.AreEqual("Keyboard", result[0].Name);

        _mockRepository.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Keyboard",
            Category = "Electronics",
            Price = 2500,
            Stock = 10
        };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Keyboard", result.Name);

        _mockRepository.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once);
    }

    [TestMethod]
    public async Task AddAsync_AddsProductThroughRepository()
    {
        // Arrange
        var product = new Product
        {
            Name = "Mouse",
            Category = "Electronics",
            Price = 1200,
            Stock = 20
        };

        Product? callbackProduct = null;

        _service.ProductChanged += changedProduct =>
        {
            callbackProduct = changedProduct;
        };

        // Act
        await _service.AddAsync(product);

        // Assert
        _mockRepository.Verify(
            repository => repository.AddAsync(product),
            Times.Once);

        Assert.AreSame(product, callbackProduct);
    }

    [TestMethod]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Updated Keyboard",
            Category = "Electronics",
            Price = 3000,
            Stock = 15
        };

        _mockRepository
            .Setup(repository => repository.UpdateAsync(product))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateAsync(product);

        // Assert
        Assert.IsTrue(result);

        _mockRepository.Verify(
            repository => repository.UpdateAsync(product),
            Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 99,
            Name = "Unknown Product",
            Category = "Other",
            Price = 100,
            Stock = 1
        };

        _mockRepository
            .Setup(repository => repository.UpdateAsync(product))
            .ReturnsAsync(false);

        // Act
        bool result = await _service.UpdateAsync(product);

        // Assert
        Assert.IsFalse(result);

        _mockRepository.Verify(
            repository => repository.UpdateAsync(product),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenProductExists_DeletesProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Keyboard",
            Category = "Electronics",
            Price = 2500,
            Stock = 10
        };

        _mockRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        _mockRepository
            .Setup(repository => repository.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.DeleteAsync(1);

        // Assert
        Assert.IsTrue(result);

        _mockRepository.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once);

        _mockRepository.Verify(
            repository => repository.DeleteAsync(1),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _mockRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        // Act
        bool result = await _service.DeleteAsync(99);

        // Assert
        Assert.IsFalse(result);

        _mockRepository.Verify(
            repository => repository.DeleteAsync(99),
            Times.Never);
    }

    [TestMethod]
    public async Task GetAllAsync_WhenCalledTwice_UsesCachedResult()
    {
        // Arrange
        IReadOnlyList<Product> products = new List<Product>
        {
            new Product
            {
                ProductId = 1,
                Name = "Keyboard",
                Category = "Electronics",
                Price = 2500,
                Stock = 10
            }
        };

        _mockRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var firstResult = await _service.GetAllAsync();
        var secondResult = await _service.GetAllAsync();

        // Assert
        Assert.AreSame(firstResult, secondResult);

        _mockRepository.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [TestMethod]
    public async Task AddAsync_InvalidatesAllProductsCache()
    {
        // Arrange
        IReadOnlyList<Product> initialProducts =
            new List<Product>
            {
            new Product { ProductId = 1, Name = "Laptop", Category = "Electronics", Price = 1000, Stock = 5 }
            };

        IReadOnlyList<Product> updatedProducts =
            new List<Product>
            {
            new Product { ProductId = 1, Name = "Laptop", Category = "Electronics", Price = 1000, Stock = 5 },
            new Product { ProductId = 2, Name = "Mouse", Category = "Electronics", Price = 25, Stock = 10 }
            };

        _mockRepository.SetupSequence(r => r.GetAllAsync())
            .ReturnsAsync(initialProducts)
            .ReturnsAsync(updatedProducts);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.GetAllAsync(); // First call: retrieves and caches initial list.

        await _service.AddAsync(
            new Product { ProductId = 2, Name = "Mouse", Category = "Electronics", Price = 25, Stock = 10 });

        var result = await _service.GetAllAsync(); // Should retrieve updated list.

        // Assert
        Assert.HasCount(2, result);

        _mockRepository.Verify(
            r => r.GetAllAsync(),
            Times.Exactly(2));
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesProductCache()
    {
        // Arrange
        var originalProduct = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1000,
            Stock = 5
        };

        var updatedProduct = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1200,
            Stock = 8
        };

        _mockRepository.SetupSequence(r => r.GetByIdAsync(1))
            .ReturnsAsync(originalProduct)
            .ReturnsAsync(updatedProduct);

        _mockRepository.Setup(r => r.UpdateAsync(updatedProduct))
            .ReturnsAsync(true);

        // Act
        await _service.GetByIdAsync(1); // Cache original product.

        bool result = await _service.UpdateAsync(updatedProduct);

        var retrievedProduct = await _service.GetByIdAsync(1);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(retrievedProduct);
        Assert.AreEqual(1200m, retrievedProduct.Price);

        _mockRepository.Verify(
            r => r.GetByIdAsync(1),
            Times.Exactly(2));
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesProductCache()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1000,
            Stock = 5
        };

        _mockRepository.SetupSequence(r => r.GetByIdAsync(1))
            .ReturnsAsync(product) // Initial retrieval.
            .ReturnsAsync(product) // ProductService checks before deletion.
            .ReturnsAsync((Product?)null); // Retrieval after deletion.

        _mockRepository.Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        await _service.GetByIdAsync(1); // Cache product.

        bool result = await _service.DeleteAsync(1);

        var retrievedProduct = await _service.GetByIdAsync(1);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(retrievedProduct);

        _mockRepository.Verify(
            r => r.GetByIdAsync(1),
            Times.Exactly(3));
    }

    [TestMethod]
    public async Task AddAsync_InvalidatesReportCaches()
    {
        // Arrange
        var cachedAveragePrices = new List<CategoryAveragePrice>
    {
        new() { Category = "Electronics", AveragePrice = 1000m }
    };

        var cachedHighestStockValue = new HighestStockValueCategory
        {
            Category = "Electronics",
            TotalStockValue = 5000m
        };

        _cache.Set(CacheKeys.AveragePriceByCategory, cachedAveragePrices);
        _cache.Set(CacheKeys.HighestStockValueCategory, cachedHighestStockValue);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1000m,
            Stock = 5
        };

        // Act
        await _service.AddAsync(product);

        // Assert
        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.AveragePriceByCategory, out _));

        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.HighestStockValueCategory, out _));
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesReportCaches()
    {
        // Arrange
        _cache.Set(
            CacheKeys.AveragePriceByCategory,
            new List<CategoryAveragePrice>());

        _cache.Set(
            CacheKeys.HighestStockValueCategory,
            new HighestStockValueCategory());

        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1200m,
            Stock = 8
        };

        _mockRepository.Setup(r => r.UpdateAsync(product))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateAsync(product);

        // Assert
        Assert.IsTrue(result);

        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.AveragePriceByCategory, out _));

        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.HighestStockValueCategory, out _));
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesReportCaches()
    {
        // Arrange
        _cache.Set(
            CacheKeys.AveragePriceByCategory,
            new List<CategoryAveragePrice>());

        _cache.Set(
            CacheKeys.HighestStockValueCategory,
            new HighestStockValueCategory());

        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Category = "Electronics",
            Price = 1000m,
            Stock = 5
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        _mockRepository.Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.DeleteAsync(1);

        // Assert
        Assert.IsTrue(result);

        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.AveragePriceByCategory, out _));

        Assert.IsFalse(_cache.TryGetValue(
            CacheKeys.HighestStockValueCategory, out _));
    }
}

