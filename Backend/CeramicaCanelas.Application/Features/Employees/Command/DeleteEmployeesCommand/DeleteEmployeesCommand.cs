using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Command.DeleteEmployeesCommand
{
    public class DeleteEmployeesCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

    }
}

