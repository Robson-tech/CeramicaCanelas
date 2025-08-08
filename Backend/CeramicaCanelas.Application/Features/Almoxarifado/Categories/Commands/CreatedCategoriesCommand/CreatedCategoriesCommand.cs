using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;


namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.CreatedCategoriesCommand
{
    public class CreatedCategoriesCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public Domain.Entities.Almoxarifado.Categories AssignToCategories()
        {
            return new Domain.Entities.Almoxarifado.Categories
            {
                Name = Name,
                Description = Description ?? "Sem descrição",
                CreatedOn = DateTime.UtcNow,
            };
        }
    }
}
