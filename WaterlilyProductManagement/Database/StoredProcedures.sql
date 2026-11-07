
USE [WaterlilyProductManagementDb];
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetAveragePriceByCategory
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Category,
        AVG(Price) AS AveragePrice
    FROM dbo.Products
    GROUP BY Category
    ORDER BY Category;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetHighestStockValueCategory
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        Category,
        SUM(Price * Stock) AS TotalStockValue
    FROM dbo.Products
    GROUP BY Category
    ORDER BY TotalStockValue DESC, Category ASC;
END;
GO
