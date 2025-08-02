using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByAccountsPayReport
{
    public class BalanceIncomeResult
    {
        public string PaymentMethod { get; set; } = "Desconhecido";
        public decimal TotalIncome { get; set; }
    }
}
