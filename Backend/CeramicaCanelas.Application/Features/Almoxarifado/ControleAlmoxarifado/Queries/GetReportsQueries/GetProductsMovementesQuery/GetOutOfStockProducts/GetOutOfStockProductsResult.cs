using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetOutOfStockProducts
{
    public class GetOutOfStockProductsResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockCurrent { get; set; }
        public int StockMinimum { get; set; }
        public DateTime? LastExit { get; set; }
        public string Status { get; set; } = "Em Falta";
    }
}
