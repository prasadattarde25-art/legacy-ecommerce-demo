namespace Ecommerce.Core.ViewModels
{
    public class ProductCardViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public decimal Price { get; set; }

        public decimal? ListPrice { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ShortDescription { get; set; }
    }
}