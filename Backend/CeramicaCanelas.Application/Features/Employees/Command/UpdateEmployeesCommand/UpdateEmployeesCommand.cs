using MediatR;
using Microsoft.AspNetCore.Http;


namespace CeramicaCanelas.Application.Features.Employees.Command.UpdateEmployeesCommand
{
    public class UpdateEmployeesCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public Domain.Enums.Positiions Positiions { get; set; }
        public IFormFile? Imagem { get; set; }

    }
}
