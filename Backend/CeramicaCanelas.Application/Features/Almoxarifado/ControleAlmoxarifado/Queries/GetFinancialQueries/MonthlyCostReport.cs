// No arquivo: MonthlyCostReport.cs
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries
{
    public class MonthlyCostReport
    {
        public required PagedResultDashboard<GetMonthlyCostByCategoryResult> PagedData { get; set; }
        public float TotalCostOverall { get; set; }
        public float AverageCostPerRecord { get; set; }
    }
}