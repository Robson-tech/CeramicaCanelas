using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Dashboard
{
    public class GetDashboardIndicatorsQuery : IRequest<GetDashboardIndicatorsResult>
    {
        public int Year { get; set; } = DateTime.UtcNow.Year; // Para filtro do gráfico de entradas
    }
}
