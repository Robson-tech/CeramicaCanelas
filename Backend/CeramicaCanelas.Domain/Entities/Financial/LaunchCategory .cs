// Local: CeramicaCanelas.Domain/Entities/LaunchCategory.cs
using CeramicaCanelas.Domain.Abstract;
using System.Collections.Generic;

namespace CeramicaCanelas.Domain.Entities.Financial
{
    public class LaunchCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Se 'true', a categoria foi "deletada" e não deve aparecer nas listagens.
        /// </summary>
        public bool IsDeleted { get; set; } = false; // <-- CAMPO ADICIONADO
    }
}