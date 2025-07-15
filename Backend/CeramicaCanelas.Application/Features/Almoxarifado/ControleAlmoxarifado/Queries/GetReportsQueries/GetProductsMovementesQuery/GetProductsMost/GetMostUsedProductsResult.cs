namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsMost
{
    public class GetMostUsedProductsResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalSaidas { get; set; }
        public DateTime? UltimaRetirada { get; set; }
        public int EstoqueAtual { get; set; }
    }
}
