namespace ristorante_backend.Models
{
    public class Menu
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }

        public List<int> DishIds { get; set; } = new List<int>(); // cambiare quando si prendono le informazioni da DB
        public List<Dish> Dishes { get; set; } = new List<Dish>();


        public Menu() { }
    }
}
