using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.DashboardFinancialSummaryResult
{
    public class MonthlyCashFlowChartItem
    {
        public string Month { get; set; } = string.Empty; // Ex: "2025-08"
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }

    public class DashboardFinancialSummaryResult
    {
        public decimal TotalIncomeYear { get; set; }
        public decimal TotalExpenseYear { get; set; }
        public decimal BalanceYear => TotalIncomeYear - TotalExpenseYear;

        public decimal TotalIncome30Days { get; set; }
        public decimal TotalExpense30Days { get; set; }

        public decimal PendingReceivables { get; set; }
        public decimal PendingPayments { get; set; }

        public decimal CurrentBalance { get; set; }
        public DateOnly? LastLaunchDate { get; set; }

        public int CustomersWithLaunches { get; set; }

        // 🔥 Gráfico mensal incorporado
        public List<MonthlyCashFlowChartItem> MonthlyChart { get; set; } = new();
    }
}
