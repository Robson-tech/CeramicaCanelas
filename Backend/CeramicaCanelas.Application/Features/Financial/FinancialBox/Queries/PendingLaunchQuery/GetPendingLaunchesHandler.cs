using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PendingLaunchQuery
{
    public class GetPagedPendingLaunchesHandler : IRequestHandler<PendingLaunchQuery, PagedResultPendingLaunch>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedPendingLaunchesHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultPendingLaunch> Handle(PendingLaunchQuery request, CancellationToken cancellationToken)
        {
            var launches = _launchRepository.QueryAllWithIncludes();

            var pending = launches
                .Where(l => l.Status == PaymentStatus.Pending)
                .AsQueryable();

            if (request.Type.HasValue)
                pending = pending.Where(l => l.Type == request.Type.Value);

            if (request.StartDate.HasValue)
                pending = pending.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                pending = pending.Where(l => l.LaunchDate <= request.EndDate.Value);

            var totalItems = pending.Count();

            var pagedItems = pending
                .OrderByDescending(l => l.LaunchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList()
                .Select(l => new PendingLaunchResult
                {
                    Id = l.Id,
                    Description = l.Description,
                    Amount = l.Amount,
                    LaunchDate = l.LaunchDate,
                    Type = l.Type,
                    CustomerName = l.Customer?.Name,
                    CategoryName = l.Category?.Name
                })
                .ToList();

            return new PagedResultPendingLaunch
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
