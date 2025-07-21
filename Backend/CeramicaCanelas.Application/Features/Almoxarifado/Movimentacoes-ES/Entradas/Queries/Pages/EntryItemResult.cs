using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Entradas.Queries.Pages
{
    public class EntryItemResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public DateTime EntryDate { get; set; }
        public string InsertedBy { get; set; } = string.Empty;

        public EntryItemResult(Domain.Entities.ProductEntry entry)
        {
            ProductName = entry.Product.Name;
            CategoryName = entry.Product.Category.Name ?? "Produto não encontrado";
            UnitPrice = entry.UnitPrice ;
            Quantity = entry.Quantity;
            EntryDate = entry.EntryDate;
            InsertedBy = entry.User?.UserName ?? "Desconhecido";
        }
    }
}