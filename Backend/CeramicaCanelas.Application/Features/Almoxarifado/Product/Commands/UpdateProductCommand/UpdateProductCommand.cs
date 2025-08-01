using CeramicaCanelas.Domain.Enums.Almoxarifado;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.UpdateProductCommand
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int StockInitial { get; set; }
        public int StockMinium { get; set; }
        public float Value { get; set; }
        public IFormFile? Imagem { get; set; }
        public bool IsReturnable { get; set; }
        public string Observation { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }

    }
}
