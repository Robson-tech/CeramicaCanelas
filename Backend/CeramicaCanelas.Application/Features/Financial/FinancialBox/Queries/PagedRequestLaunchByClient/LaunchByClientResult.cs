using CeramicaCanelas.Domain.Enums.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class LaunchByClientResult
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public LaunchType Type { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime LaunchDate { get; set; }
    }
}
