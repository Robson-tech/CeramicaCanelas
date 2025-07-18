using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsPeriods
{
    public class GetProductReportQuery : IRequest<List<GetProductReportResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchProduct { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
