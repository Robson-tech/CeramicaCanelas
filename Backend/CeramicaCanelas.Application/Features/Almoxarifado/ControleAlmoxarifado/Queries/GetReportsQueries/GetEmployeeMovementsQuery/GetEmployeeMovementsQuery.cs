using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery
{
    public class GetEmployeeMovementsQuery : IRequest<List<GetEmployeeMovementsResult>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SearchEmployee { get; set; } // busca por nome
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
