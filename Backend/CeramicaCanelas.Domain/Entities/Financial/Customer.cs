// Local: CeramicaCanelas.Domain/Entities/Customer.cs
using CeramicaCanelas.Domain.Abstract;
using System.Collections.Generic;

namespace CeramicaCanelas.Domain.Entities.Financial
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Se 'true', o cliente foi "deletado" e não deve aparecer nas listagens.
        /// </summary>
        public bool IsDeleted { get; set; } = false; 

        public ICollection<Launch> Launches { get; set; } = new List<Launch>();
    }
}