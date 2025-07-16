using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Dashboard
{
    public class GetDashboardIndicatorsResult
    {
        public int[] EntradasPorMes { get; set; } = new int[12];
        public int ProdutosNaoDevolvidos { get; set; }
        public int TotalProdutosCadastrados { get; set; }
        public double PorcentagemDevolucaoGeral { get; set; }
        public int ProdutosComFuncionario { get; set; }
        public int TotalProdutosEstoque { get; set; }
        public float GastoTotalComprasMes { get; set; }
        public int AlertasEstoqueMinimo { get; set; }
    }
}
