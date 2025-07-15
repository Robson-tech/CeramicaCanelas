using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery
{
    public class GetEmployeeMovementsResult
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityRetirada { get; set; }
        public int QuantityDevolvida { get; set; }
        public int QuantityPendente { get; set; } // <- adicionada ou mantida
        public DateTime DataRetirada { get; set; }
    }


}
