namespace CeramicaCanelas.Application.Features.Categories.Queries.GetAllCategoriesQueries
{
    public class GetAllCategoriesResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public GetAllCategoriesResult(Domain.Entities.Categories categories)
        {
            if(categories == null)
            {
                throw new ArgumentNullException(nameof(categories), "Categories cannot be null");
            }

            Id = categories.Id;
            Name = categories.Name;
            Description = categories.Description;
            ImageUrl = categories.ImageUrl!;
            

        }
    }
}
