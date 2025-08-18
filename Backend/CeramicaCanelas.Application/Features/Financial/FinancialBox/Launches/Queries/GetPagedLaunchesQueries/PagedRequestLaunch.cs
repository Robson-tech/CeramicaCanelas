using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Queries.GetPagedLaunchesQueries
{
    public class PagedRequestLaunch : IRequest<PagedResultLaunch>
    {
        // Filtros específicos para Lançamentos
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Para buscar na descrição
        public string? SearchCategoryOrCustomer { get; set; } // Para buscar na categoria
        public LaunchType? Type { get; set; } // Filtrar por Entrada ou Saída
        public DateOnly? StartDate { get; set; } // Filtro de data inicial
        public DateOnly? EndDate { get; set; } // Filtro de data final
        public Guid? CategoryId { get; set; } // Filtrar por categoria
        public Guid? CustomerId { get; set; } // Filtrar por cliente
        public PaymentStatus? Status { get; set; } // Filtrar por status

    }
}
