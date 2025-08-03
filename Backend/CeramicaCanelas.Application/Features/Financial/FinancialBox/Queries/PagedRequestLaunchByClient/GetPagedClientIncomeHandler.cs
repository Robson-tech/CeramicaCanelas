using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    // Handler para a nova requisição
    public class GetPagedClientIncomeHandler : IRequestHandler<PagedClientIncomeRequest, PagedClientIncomeResult>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedClientIncomeHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedClientIncomeResult> Handle(PagedClientIncomeRequest request, CancellationToken cancellationToken)
        {
            // 1. Inicia a consulta, incluindo o Cliente para podermos usar o nome.
            //    Filtra apenas por entradas (Income) e lançamentos que tenham cliente.
            var query = _launchRepository.QueryAllWithIncludes() // ESSENCIAL que isso faça .Include(l => l.Customer)
                .Where(l => l.Type == LaunchType.Income && l.CustomerId != null);

            // 2. Aplica filtro de data, se fornecido.
            //    Lembre-se de garantir que as datas sejam UTC para evitar erros!
            if (request.StartDate.HasValue)
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value.ToUniversalTime());

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate <= request.EndDate.Value.ToUniversalTime());

            // 3. A MÁGICA ACONTECE AQUI: Agrupamento por Cliente
            //    Agrupamos todos os lançamentos por ID e Nome do Cliente.
            var groupedQuery = query.GroupBy(
                l => new { l.CustomerId, l.Customer!.Name }, // Chave de agrupamento
                (key, group) => new ClientIncomeSummaryResult // Projeta o resultado do grupo
                {
                    CustomerId = key.CustomerId!.Value,
                    CustomerName = key.Name,
                    TotalAmount = group.Sum(l => l.Amount) // Soma os valores do grupo
                });

            // 4. Aplica filtro de busca por nome DEPOIS de agrupar.
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                groupedQuery = groupedQuery.Where(c => c.CustomerName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Ordenação do resultado agrupado.
            groupedQuery = request.OrderBy.ToLower() switch
            {
                "total" => request.Ascending
                                ? groupedQuery.OrderBy(c => c.TotalAmount)
                                : groupedQuery.OrderByDescending(c => c.TotalAmount),
                _ => request.Ascending
                                ? groupedQuery.OrderBy(c => c.CustomerName)
                                : groupedQuery.OrderByDescending(c => c.CustomerName),
            };

            // 6. Calcula o total de itens (clientes únicos) para a paginação.
            var totalItems = await groupedQuery.CountAsync(cancellationToken);

            // 7. Aplica a paginação e executa a consulta, trazendo os dados para a memória.
            var pagedItems = await groupedQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // 8. Retorna o resultado final e paginado.
            return new PagedClientIncomeResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}

