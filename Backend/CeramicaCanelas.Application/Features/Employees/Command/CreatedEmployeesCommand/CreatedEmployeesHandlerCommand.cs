using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Command.CreatedEmployeesCommand
{
    // Nome da classe ajustado para consistência
    public class CreatedEmployeeCommandHandler : IRequestHandler<CreatedEmployeesCommand, Unit>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ILogged _logged;

        // Caminho absoluto no servidor VPS onde as imagens dos funcionários serão salvas
        private const string PastaBaseVps = "/var/www/ceramicacanelas/almoxarifado/employees/images";

        // Caminho público que será exposto no navegador
        private const string UrlBase = "https://api.ceramicacanelas.shop/almoxarifado/employees/images/";

        public CreatedEmployeeCommandHandler(IEmployeesRepository employeeRepository, ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreatedEmployeesCommand request, CancellationToken cancellationToken)
        {
            // Valida se o usuário está autenticado
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            // Valida o conteúdo da requisição
            await ValidateEmployee(request, cancellationToken);

            string? imageUrl = null;

            if (request.Imagem != null)
            {
                // Garante que o diretório exista
                Directory.CreateDirectory(PastaBaseVps);

                var nomeArquivo = $"{Guid.NewGuid()}_{request.Imagem.FileName}";
                var caminhoAbsoluto = Path.Combine(PastaBaseVps, nomeArquivo);

                // Salva o arquivo na VPS
                using var stream = new FileStream(caminhoAbsoluto, FileMode.Create);
                await request.Imagem.CopyToAsync(stream);

                // Cria URL pública de acesso à imagem
                imageUrl = $"{UrlBase}{nomeArquivo}";
            }
            else
            {
                // Se não houver imagem, define uma URL padrão ou nula
                var ImageDefault = "imageDefault.jpg"; // Nome do arquivo padrão
                imageUrl = $"{UrlBase}{ImageDefault}"; // ou defina uma URL padrão se necessário
            }

            var employee = request.AssignToEmployee();
            employee.ImageUrl = imageUrl;

            await _employeeRepository.CreateAsync(employee, cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateEmployee(CreatedEmployeesCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedEmployeesValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                throw new BadRequestException(result);
        }
    }
}