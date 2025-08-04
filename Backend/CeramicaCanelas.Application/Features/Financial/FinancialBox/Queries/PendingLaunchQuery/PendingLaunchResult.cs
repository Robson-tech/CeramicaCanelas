using CeramicaCanelas.Domain.Enums.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PendingLaunchQuery
{
    public class PendingLaunchResult
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly LaunchDate { get; set; }
        public LaunchType Type { get; set; }
        public string? CustomerName { get; set; }
        public string? CategoryName { get; set; }
    }
}
