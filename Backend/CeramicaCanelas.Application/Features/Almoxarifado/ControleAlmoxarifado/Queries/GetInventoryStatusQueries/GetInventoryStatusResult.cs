using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetInventoryStatusQueries
{
    public class GetInventoryStatusResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int StockInitial { get; set; }
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int StockCurrent { get; set; }
        public int StockMinimum { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime? LastMovement { get; set; }
    }
}
