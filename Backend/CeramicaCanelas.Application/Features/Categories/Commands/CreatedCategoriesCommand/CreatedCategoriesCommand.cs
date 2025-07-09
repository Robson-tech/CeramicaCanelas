using CeramicaCanelas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;


namespace CeramicaCanelas.Application.Features.Categories.Commands.CreatedCategoriesCommand
{
    public class CreatedCategoriesCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public IFormFile? Imagem { get; set; }

        public Domain.Entities.Categories AssignToCategories()
        {
            return new Domain.Entities.Categories
            {
                Name = Name,
                Description = Description,
                CreatedOn = DateTime.UtcNow,
            };
        }
    }
}
