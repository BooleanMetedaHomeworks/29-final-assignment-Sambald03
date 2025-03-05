using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ristorante_frontend.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<int> DishIds { get; set; } = new List<int>();
        public List<Dish> Dishes { get; set; } = new List<Dish>();
    }
}
