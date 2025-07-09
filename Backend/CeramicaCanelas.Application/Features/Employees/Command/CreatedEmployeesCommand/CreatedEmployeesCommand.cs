using MediatR;
using Microsoft.AspNetCore.Http;

namespace CeramicaCanelas.Application.Features.Employees.Command.CreatedEmployeesCommand
{
    public class CreatedEmployeesCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public Domain.Enums.Positiions Positiions { get; set; }
        public IFormFile? Imagem { get; set; }


        public Domain.Entities.Employee AssignToEmployee()
        {
            return new Domain.Entities.Employee
            {
                Name = Name,
                CPF = CPF,
                Positiions = Positiions,
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };
        }

    }
}
