using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.DeleteMovEntradasProductsCommand
{
    public class DeleteMovEntradasProductsCommandHandler : IRequestHandler<DeleteMovEntradasProductsCommand, Unit>
    {
        private readonly IMovimentacaoEntradasProductsRepository _repository;
        private readonly ILogged _logged;

        public DeleteMovEntradasProductsCommandHandler(IMovimentacaoEntradasProductsRepository repository, ILogged logged)
        {
            _repository = repository;
            _logged = logged;
        }

        public async Task<Unit> Handle(DeleteMovEntradasProductsCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var movimentacaoES = await _repository.GetByIdAsync(command.Id);

            if (movimentacaoES == null)
                throw new BadRequestException("Movimentação de entrada não encontrada.");

            await _repository.Delete(movimentacaoES);

            return Unit.Value;
        }

    }

}

