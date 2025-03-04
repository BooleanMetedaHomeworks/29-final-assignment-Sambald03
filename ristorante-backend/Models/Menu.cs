namespace ristorante_backend.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<int> Dish_ids { get; set; } = new List<int>();
        public List<Dish> Dishes { get; set; } = new List<Dish>();


        public Menu() { }

        /*
        public Menu(string name)
        {
            this.Name = name;
        }

        public Menu(int id, string name) : this (name)
        {
            this.Id = id;
        }
        */
    }
}
