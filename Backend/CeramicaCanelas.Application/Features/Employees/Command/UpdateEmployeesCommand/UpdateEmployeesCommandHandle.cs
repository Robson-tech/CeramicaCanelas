using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities; // Adicionado para usar a entidade Employee
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Command.UpdateEmployeesCommand
{
    // Nome da classe ajustado para consistência
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeesCommand, Unit>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ILogged _logged;

        // Caminhos padronizados como constantes na classe
        private const string PastaBaseVps = "/var/www/ceramicacanelas/almoxarifado/employees/images";
        private const string UrlBase = "https://api.ceramicacanelas.shop/almoxarifado/employees/images/";

        public UpdateEmployeeCommandHandler(IEmployeesRepository employeeRepository, ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(UpdateEmployeesCommand request, CancellationToken cancellationToken)
        {
            // 1. Validação de autenticação
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            // 2. Validação da requisição
            await ValidateRequest(request, cancellationToken);

            // 3. Busca o funcionário existente no banco de dados
            var employeeToUpdate = await _employeeRepository.GetByIdAsync(request.Id);
            if (employeeToUpdate == null)
                throw new BadRequestException($"Funcionário com o ID {request.Id} não foi encontrado.");

            string? newImageUrl = employeeToUpdate.ImageUrl; // Mantém a imagem atual por padrão

            // 4. Lógica para lidar com a nova imagem (se enviada)
            if (request.Imagem != null)
            {
                // Deleta a imagem antiga, se existir
                if (!string.IsNullOrEmpty(employeeToUpdate.ImageUrl))
                {
                    var nomeArquivoAntigo = Path.GetFileName(employeeToUpdate.ImageUrl);
                    var caminhoAbsolutoAntigo = Path.Combine(PastaBaseVps, nomeArquivoAntigo);
                    if (File.Exists(caminhoAbsolutoAntigo))
                    {
                        File.Delete(caminhoAbsolutoAntigo);
                    }
                }

                // Garante que o diretório exista
                Directory.CreateDirectory(PastaBaseVps);

                // Salva a nova imagem
                var nomeNovoArquivo = $"{Guid.NewGuid()}_{request.Imagem.FileName}";
                var caminhoAbsolutoNovo = Path.Combine(PastaBaseVps, nomeNovoArquivo);

                using var stream = new FileStream(caminhoAbsolutoNovo, FileMode.Create);
                await request.Imagem.CopyToAsync(stream);

                newImageUrl = $"{UrlBase}{nomeNovoArquivo}";
            }
            else
            {
                // Se não houver imagem, define uma URL padrão ou nula
                var ImageDefault = "imageDefault.jpg"; // Nome do arquivo padrão
                newImageUrl = $"{UrlBase}{ImageDefault}"; // ou defina uma URL padrão se necessário
            }

            // 5. Mapeia os dados da requisição para a entidade
            // ATENÇÃO: Verifique se a propriedade "Positiions" não deveria ser "Positions" (sem o 'i' extra)
            employeeToUpdate.Name = request.Name;
            employeeToUpdate.CPF = request.CPF;
            employeeToUpdate.Positiions = request.Positiions; // Possível erro de digitação aqui
            employeeToUpdate.ImageUrl = newImageUrl;
            employeeToUpdate.ModifiedOn = DateTime.UtcNow;

            // 6. Persiste as alterações
            await _employeeRepository.Update(employeeToUpdate);

            return Unit.Value;
        }

        private async Task ValidateRequest(UpdateEmployeesCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateEmployeesCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                throw new BadRequestException(result);
        }
    }
}