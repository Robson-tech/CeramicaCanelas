using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Queries.GetAllProductsEntryQueries
{
    public class GetAllProductsEntryQueriesResult
    {
        public string NameProduct { get; set; } = string.Empty;
        public string DescriptionProduct { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public int Quantity { get; set; }
        public string NameOperator { get; set; } = string.Empty;
        public float UnitPrice { get; set; }
        public DateTime CreatedOn { get; set; }

        public GetAllProductsEntryQueriesResult(ProductEntry productEntry)
        {
            NameProduct = productEntry.Product?.Name ?? "Produto não encontrado";
            DescriptionProduct = productEntry.Product?.Observation ?? string.Empty;
            EntryDate = productEntry.EntryDate;
            NameOperator = productEntry.User.Name;
            Quantity = productEntry.Quantity;
            UnitPrice = productEntry.UnitPrice;
            CreatedOn = productEntry.CreatedOn;
            

        }

    }
}
