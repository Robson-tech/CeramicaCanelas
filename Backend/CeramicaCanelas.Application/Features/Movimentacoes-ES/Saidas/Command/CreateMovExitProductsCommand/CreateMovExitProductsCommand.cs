

using MediatR;


namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.CreateMovExitProductsCommand
{
    public class CreateMovExitProductsCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public Guid EmployeeId {  get; set; }
        public int Quantity { get; set; }
        public bool IsReturnable { get; set; }
        public string? Observation { get; set; }
        public Domain.Entities.ProductExit AssignToProductsExit()
        {
            return new Domain.Entities.ProductExit
            {
                ExitDate = DateTime.UtcNow,
                ProductId = ProductId,
                EmployeeId = EmployeeId,
                Quantity = Quantity,
                IsReturnable = IsReturnable,
                Observation = Observation,
                IsReturned = false,
                ReturnDate = null,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow

            };
        }
    }
}
