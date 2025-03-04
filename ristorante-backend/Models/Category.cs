namespace ristorante_backend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public Category() { }

        /*
        public Category(string name)
        {
            this.Name = name;
        }

        public Category(int id, string name) : this (name)
        {
            this.Id = id;
        }
        */
    }
}
