using Microsoft.AspNetCore.Mvc;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Models;
using WaterlilyProductManagement.ViewModels.Products;

namespace WaterlilyProductManagement.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();

        var viewModel = products.Select(product =>
            new ProductListItemViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock
            }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var product = new Product
        {
            Name = viewModel.Name,
            Category = viewModel.Category,
            Price = viewModel.Price!.Value,
            Stock = viewModel.Stock!.Value
        };

        await _productService.AddAsync(product);

        TempData["SuccessMessage"] = "Product created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        var viewModel = new ProductEditViewModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ProductEditViewModel viewModel)
    {
        if (id != viewModel.ProductId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var product = new Product
        {
            ProductId = viewModel.ProductId,
            Name = viewModel.Name,
            Category = viewModel.Category,
            Price = viewModel.Price!.Value,
            Stock = viewModel.Stock!.Value
        };

        var isUpdated = await _productService.UpdateAsync(product);

        if (!isUpdated)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Product updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        var viewModel = new ProductListItemViewModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var isDeleted = await _productService.DeleteAsync(id);

        if (!isDeleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Product deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}
