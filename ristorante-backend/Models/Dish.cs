namespace ristorante_backend.Models
{
    public class Dish
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string? Description { get; set; } = null;
        public decimal Price { get; set; } = 0.0m;

        public int? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;

        public List<int> MenuIds { get; set; } = new List<int>();
        public List<Menu> Menus { get; set; } = new List<Menu>();


        public Dish() { }
    }
}
