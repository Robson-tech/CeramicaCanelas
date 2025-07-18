using CeramicaCanelas.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Employees.Queries.Pages
{
    public class PagedResultEmployees
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<EmployeeResult> Items { get; set; } = new();
    }

    public class EmployeeResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Positiions? Positions { get; set; }
        public string CPF { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public EmployeeResult(Domain.Entities.Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Positions = employee.Positiions;
            CPF = employee.CPF;
            ImageUrl = employee.ImageUrl ?? string.Empty;
            IsActive = employee.IsActive;
        }
    }
}
