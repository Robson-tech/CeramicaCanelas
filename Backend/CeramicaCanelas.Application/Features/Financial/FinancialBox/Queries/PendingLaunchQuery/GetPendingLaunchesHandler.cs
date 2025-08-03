using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PendingLaunchQuery
{
    public class GetPendingLaunchesHandler : IRequestHandler<PendingLaunchQuery, List<PendingLaunchResult>>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPendingLaunchesHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<List<PendingLaunchResult>> Handle(PendingLaunchQuery request, CancellationToken cancellationToken)
        {
            var launches = await _launchRepository.GetAllAsync();

            var pending = launches
                .Where(l => l.Status == PaymentStatus.Pending)
                .AsQueryable();

            if (request.Type.HasValue)
                pending = pending.Where(l => l.Type == request.Type.Value);

            if (request.StartDate.HasValue)
                pending = pending.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                pending = pending.Where(l => l.LaunchDate <= request.EndDate.Value);

            return pending
                .OrderBy(l => l.LaunchDate)
                .Select(l => new PendingLaunchResult
                {
                    Id = l.Id,
                    Description = l.Description,
                    Amount = l.Amount,
                    LaunchDate = l.LaunchDate,
                    Type = l.Type,
                    CustomerName = l.Customer?.Name,
                    CategoryName = l.Category?.Name
                }).ToList();
        }
    }
}
