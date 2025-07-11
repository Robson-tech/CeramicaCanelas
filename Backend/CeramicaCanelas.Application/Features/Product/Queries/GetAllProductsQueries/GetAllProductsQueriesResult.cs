using CeramicaCanelas.Domain.Enums;

public class GetAllProductsQueriesResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public int StockInitial { get; set; }
    public int StockMinium { get; set; }
    public int StockCurrent { get; set; }
    public float ValueUnit { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsReturnable { get; set; }
    public string? Observation { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public GetAllProductsQueriesResult(CeramicaCanelas.Domain.Entities.Products product)
    {
        Id = product.Id;
        Code = product.Code;
        Name = product.Name;
        UnitOfMeasure = product.UnitOfMeasure;
        StockInitial = product.StockInitial;
        StockMinium = product.StockMinium;
        StockCurrent = product.StockCurrent;
        ValueUnit = product.ValueUnit;
        ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
            ? "https://localhost:7014/products/images/default.png"
            : product.ImageUrl;
        IsReturnable = product.IsReturnable;
        Observation = product.Observation;
        CategoryId = product.CategoryId;
        CategoryName = product.Category?.Name ?? "Sem categoria";
    }
}
