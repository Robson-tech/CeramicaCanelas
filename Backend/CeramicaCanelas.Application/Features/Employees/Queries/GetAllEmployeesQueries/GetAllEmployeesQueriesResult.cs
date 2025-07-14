using CeramicaCanelas.Domain.Enums;


namespace CeramicaCanelas.Application.Features.Employees.Queries.GetAllEmployeesQueries
{
    public class GetAllEmployeesQueriesResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public Positiions Positiions { get; set; }
        public string? CPF { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public GetAllEmployeesQueriesResult(Domain.Entities.Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            
            Id = employee.Id;
            Name = employee.Name;
            Positiions = employee.Positiions;
            CPF = employee.CPF;
            ImageUrl = employee.ImageUrl ?? string.Empty;
            if (string.IsNullOrEmpty(ImageUrl))
            {
                ImageUrl = "https://localhost:7014/employees/images/default.png"; // Default image URL
            }
        }
    }
}
