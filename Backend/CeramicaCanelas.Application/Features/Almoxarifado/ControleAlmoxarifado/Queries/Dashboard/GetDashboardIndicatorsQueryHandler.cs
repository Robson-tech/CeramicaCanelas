using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Dashboard
{
    public class GetDashboardIndicatorsQueryHandler : IRequestHandler<GetDashboardIndicatorsQuery, GetDashboardIndicatorsResult>
    {
        private readonly IProductRepository _productRepo;
        private readonly IMovEntryProductsRepository _entryRepo;
        private readonly IMovExitProductsRepository _exitRepo;

        public GetDashboardIndicatorsQueryHandler(
            IProductRepository productRepo,
            IMovEntryProductsRepository entryRepo,
            IMovExitProductsRepository exitRepo)
        {
            _productRepo = productRepo;
            _entryRepo = entryRepo;
            _exitRepo = exitRepo;
        }

        public async Task<GetDashboardIndicatorsResult> Handle(GetDashboardIndicatorsQuery request, CancellationToken cancellationToken)
        {
            var produtos = await _productRepo.GetAllProductsAsync();
            var entradas = await _entryRepo.GetAllAsync();
            var saidas = await _exitRepo.GetAllAsync();

            var entradasPorMes = new int[12];
            foreach (var entrada in entradas.Where(e => e.EntryDate.Year == request.Year))
                entradasPorMes[entrada.EntryDate.Month - 1] += entrada.Quantity;

            int produtosNaoDevolvidos = saidas
                .Where(s => s.IsReturnable && s.Quantity > s.ReturnedQuantity)
                .Sum(s => s.Quantity - s.ReturnedQuantity);

            int totalProdutosCadastrados = produtos.Count();

            double percentualDevolucao = saidas
                .Where(s => s.IsReturnable)
                .Select(s => s.ReturnedQuantity / (double)s.Quantity)
                .DefaultIfEmpty(1.0)
                .Average() * 100;

            int produtosComFuncionario = saidas.Select(s => s.ProductId).Distinct().Count();

            int totalProdutosEstoque = produtos.Sum(p => p.StockCurrent);

            float gastoMesAtual = produtos
                .Where(p => p.ModifiedOn.Month == DateTime.UtcNow.Month && p.ModifiedOn.Year == DateTime.UtcNow.Year)
                .Sum(p => p.ValueTotal);

            int alertasEstoque = produtos.Count(p => p.StockCurrent < p.StockMinium);

            return new GetDashboardIndicatorsResult
            {
                EntradasPorMes = entradasPorMes,
                ProdutosNaoDevolvidos = produtosNaoDevolvidos,
                TotalProdutosCadastrados = totalProdutosCadastrados,
                PorcentagemDevolucaoGeral = Math.Round(percentualDevolucao, 2),
                ProdutosComFuncionario = produtosComFuncionario,
                TotalProdutosEstoque = totalProdutosEstoque,
                GastoTotalComprasMes = gastoMesAtual,
                AlertasEstoqueMinimo = alertasEstoque
            };
        }
    }

}
