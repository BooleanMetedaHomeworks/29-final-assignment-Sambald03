﻿namespace ristorante_backend.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;


        public Dish() { }
    }
}
