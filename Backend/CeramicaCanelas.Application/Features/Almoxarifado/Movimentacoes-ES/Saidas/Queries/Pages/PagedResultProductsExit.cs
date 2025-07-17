using CeramicaCanelas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Saidas.Queries.Pages
{
    public class PagedResultProductsExit
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<ExitItemResult> Items { get; set; } = [];
    }

    public class ExitItemResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public DateTime ExitDate { get; set; }
        public bool IsReturnable { get; set; }
        public string InsertedBy { get; set; } = string.Empty;

        public ExitItemResult(ProductExit exit)
        {
            ProductName = exit.Product?.Name ?? "Desconhecido";
            EmployeeName = exit.Employee?.Name ?? "Desconhecido";
            Quantity = exit.Quantity;
            ReturnedQuantity = exit.ReturnedQuantity;
            ExitDate = exit.ExitDate;
            IsReturnable = exit.IsReturnable;
            InsertedBy = exit.User?.UserName ?? "Desconhecido";
        }
    }
}
