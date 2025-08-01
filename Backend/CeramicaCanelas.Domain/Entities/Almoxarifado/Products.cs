using CeramicaCanelas.Domain.Abstract;
using CeramicaCanelas.Domain.Enums.Almoxarifado;

namespace CeramicaCanelas.Domain.Entities.Almoxarifado
{
    public class Products : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } =  string.Empty;
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int StockInitial {  get; set; }
        public int StockMinium { get; set; }
        public int StockCurrent { get; set; }
        public float ValueTotal { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsReturnable { get; set; }
        public string? Observation { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public Categories? Category { get; set; }


    }

}
