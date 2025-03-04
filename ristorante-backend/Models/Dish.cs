namespace ristorante_backend.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? Category_id { get; set; }
        public Category? Category { get; set; }

        public List<int> Menu_ids { get; set; } = new List<int>();
        public List<Menu> Menus { get; set; } = new List<Menu>();


        public Dish() { }

        /*
        public Dish(string name, decimal price, string? description = null)
        {
            this.Name = name;
            this.Price = price;
            this.Description = description;
        }

        public Dish(int id, string name, decimal price, string? description = null) : this (name, price, description)
        {
            this.Id = id;
        }
        */
    }
}
