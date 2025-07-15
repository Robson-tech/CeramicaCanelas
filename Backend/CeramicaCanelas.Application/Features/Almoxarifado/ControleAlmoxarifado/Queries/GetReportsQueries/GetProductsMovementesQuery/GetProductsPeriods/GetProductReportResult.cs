using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsPeriods
{
    public class GetProductReportResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int StockCurrent { get; set; }
        public int StockMinimum { get; set; }
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int PendingReturns { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public DateTime? LastMovementDate { get; set; }
    }
}
