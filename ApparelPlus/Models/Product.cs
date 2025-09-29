namespace ApparelPlus.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string? Image { get; set;  }

        // FK ref to parent Category
        public int CategoryId { get; set; }

        // parent ref to Category in which product belongs
        public Category? Category { get; set; }
    }
}
