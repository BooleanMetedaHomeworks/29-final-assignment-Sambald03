namespace ristorante_backend.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public List<int> MenuIds { get; set; } = new List<int>();
        public List<Menu> Menus { get; set; } = new List<Menu>();


        public Dish() { }
    }
}
