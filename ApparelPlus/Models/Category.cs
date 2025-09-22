namespace ApparelPlus.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // child ref to Products: 1 Category can have Many Products
        // ? is required so we can first add the parent
        public List<Product>? Products { get; set; }
    }
}
