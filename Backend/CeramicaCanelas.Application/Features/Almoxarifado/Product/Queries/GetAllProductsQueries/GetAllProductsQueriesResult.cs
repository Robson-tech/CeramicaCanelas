using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Enums.Almoxarifado;

public class GetAllProductsQueriesResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public int StockInitial { get; set; }
    public int StockMinium { get; set; }
    public int StockCurrent { get; set; }
    public float Value { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsReturnable { get; set; }
    public string? Observation { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public GetAllProductsQueriesResult(Products product)
    {
        Id = product.Id;
        Code = product.Code;
        Name = product.Name;
        UnitOfMeasure = product.UnitOfMeasure;
        StockInitial = product.StockInitial;
        StockMinium = product.StockMinium;
        StockCurrent = product.StockCurrent;
        Value = product.ValueTotal;
        ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
            ? "https://localhost:7014/products/images/default.png"
            : product.ImageUrl;
        IsReturnable = product.IsReturnable;
        Observation = product.Observation;
        CategoryId = product.CategoryId ?? Guid.Empty;
        CategoryName = product.Category?.Name ?? "Sem categoria";
    }
}
