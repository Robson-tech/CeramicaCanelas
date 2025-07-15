using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetReportsEmployeesQueries
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
