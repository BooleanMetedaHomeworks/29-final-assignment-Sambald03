namespace ristorante_backend.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<int> DishIds { get; set; } = new List<int>();
        public List<Dish> Dishes { get; set; } = new List<Dish>();


        public Menu() { }
    }
}
