using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries
{
    using System;

    namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries
    {
        public class GetAllProductsExitQueriesResult
        {
            public string NameProduct { get; set; } = string.Empty;
            public string NameEmployee { get; set; } = string.Empty;
            public DateTime ExitDate { get; set; }
            public int Quantity { get; set; }
            public bool IsReturnable { get; set; }
            public bool IsReturned { get; set; }
            public DateTime? ReturnDate { get; set; }
            public string? Observation { get; set; }
            public string NameOperator { get; set; } = string.Empty;

            public GetAllProductsExitQueriesResult(Domain.Entities.ProductExit productExit)
            {
                NameProduct = productExit.Product?.Name ?? "Desconhecido";
                NameEmployee = productExit.Employee?.Name ?? "Desconhecido";
                ExitDate = productExit.ExitDate;
                Quantity = productExit.Quantity;
                IsReturnable = productExit.IsReturnable;
                IsReturned = productExit.IsReturned;
                ReturnDate = productExit.ReturnDate;
                Observation = productExit.Observation;
                NameOperator = productExit.User.Name ?? "Desconhecido";
            }
        }
    }

}
