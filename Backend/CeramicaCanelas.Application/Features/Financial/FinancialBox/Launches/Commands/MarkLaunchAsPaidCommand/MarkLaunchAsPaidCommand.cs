using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.MarkLaunchAsPaidCommand
{
    public class MarkLaunchAsPaidCommand : IRequest<bool>
    {
        public Guid LaunchId { get; set; }
    }
}
