namespace WaterlilyProductManagement.Constants;

public static class CacheKeys
{
    // Product cache keys
    public const string AllProducts = "Products_All";

    public static string ProductById(int id)
        => $"Product_{id}";

    // Report cache keys
    public const string AveragePriceByCategory =
        "Report_AveragePriceByCategory";

    public const string HighestStockValueCategory =
        "Report_HighestStockValueCategory";
}
