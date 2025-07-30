using CeramicaCanelas.Domain.Abstract;

namespace CeramicaCanelas.Domain.Entities.Almoxarifado
{
    public class ProductExit : BaseEntity
    {
        public Guid? ProductId { get; set; }
        public Products? Product { get; set; } = null!;
        public string NameProduct { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; } = null!;
        public string EmployeeName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public User? User { get; set; } = null!;
        public string NameOperator { get; set; } = string.Empty;

        public DateTime ExitDate { get; set; }
        public int Quantity { get; set; }
        public int ReturnedQuantity { get; set; } = 0; // Total devolvido


        public bool IsReturnable { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string? Observation { get; set; }
    }

}
