using CarService.Core.Abstract;

namespace CarService.Entities.Entities
{
    public class Car : IEntity
    {
        public int Id { get; set; }
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public string? ImageUrl { get; set; }
        public int? ModelYear { get; set; }

    }
}
