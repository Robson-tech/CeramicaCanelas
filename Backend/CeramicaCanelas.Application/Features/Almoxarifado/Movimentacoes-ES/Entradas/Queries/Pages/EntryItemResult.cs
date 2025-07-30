using CeramicaCanelas.Domain.Entities.Almoxarifado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Entradas.Queries.Pages
{
    public class EntryItemResult
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public DateTime EntryDate { get; set; }
        public string InsertedBy { get; set; } = string.Empty;

        public EntryItemResult(ProductEntry entry)
        {
            Id = entry.Id;
            ProductName = entry.NameProduct ?? "Produto não encontrado";
            SupplierId = entry.SupplierId ?? Guid.Empty;
            CategoryName = entry.NameCategory ?? "Produto desconhecido";
            SupplierName = entry.NameSupplier ?? "Fornecedor não encontrado";
            UnitPrice = entry.UnitPrice ;
            Quantity = entry.Quantity;
            EntryDate = entry.EntryDate;
            InsertedBy = entry.NameOperator ?? "Desconhecido";
        }
    }
}