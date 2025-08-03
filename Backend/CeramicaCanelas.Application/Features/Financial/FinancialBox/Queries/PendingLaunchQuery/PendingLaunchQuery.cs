using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PendingLaunchQuery
{
    public class PendingLaunchQuery : IRequest<List<PendingLaunchResult>>
    {
        public LaunchType? Type { get; set; } // Filtro opcional
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
