using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries
{
    public class GetMonthlyCostByCategoryResult
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public float TotalCost { get; set; }
    }

}
