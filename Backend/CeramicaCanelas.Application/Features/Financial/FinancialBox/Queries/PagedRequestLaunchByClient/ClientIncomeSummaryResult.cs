using CeramicaCanelas.Domain.Enums.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class ClientIncomeSummaryResult
    {
        // --- Informações Básicas ---
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        // --- Métricas de Valor ---
        /// <summary>
        /// A soma total de todas as entradas (compras) do cliente no período.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// O valor médio gasto por compra (TotalAmount / QuantidadeDeCompras).
        /// </summary>
        public decimal TicketMedio { get; set; }

        /// <summary>
        /// A soma de todos os lançamentos que ainda estão com status "Pendente".
        /// </summary>
        public decimal ValorPendente { get; set; }

        // --- Métricas de Comportamento ---
        /// <summary>
        /// O número total de compras (lançamentos de entrada) realizadas.
        /// </summary>
        public int QuantidadeDeCompras { get; set; }

        /// <summary>
        /// A data da compra mais recente do cliente.
        /// </summary>
        public DateOnly DataDaUltimaCompra { get; set; }
    }
}
