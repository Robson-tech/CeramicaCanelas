using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.MarkLaunchAsPaidCommand
{
    public class MarkLaunchAsPaidHandler : IRequestHandler<MarkLaunchAsPaidCommand, bool>
    {
        private readonly ILaunchRepository _launchRepository;

        public MarkLaunchAsPaidHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<bool> Handle(MarkLaunchAsPaidCommand request, CancellationToken cancellationToken)
        {
            var launch = await _launchRepository.GetByIdAsync(request.LaunchId);
            if (launch == null || launch.Status == PaymentStatus.Paid)
                return false;

            launch.Status = PaymentStatus.Paid;
            await _launchRepository.SaveChangesAsync();
            return true;
        }
    }
}
