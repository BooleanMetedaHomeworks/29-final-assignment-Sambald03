using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using ristorante_backend.Models;
using static ristorante_backend.DB;

namespace ristorante_backend.Repositories
{
    public class DishRepository
    {
        public async Task<List<Dish>> GetAllDishes()
        {
            string query = @$"SELECT p.*, c.Id AS CategoryId, c.Name AS CategoryName, i.Id AS IngredientId, i.Name AS IngredientName
                           FROM Pizzas p
                           LEFT JOIN Categories c ON p.CategoryId = c.Id
                           LEFT JOIN PizzaIngredient pi ON p.Id = pi.PizzaId
                           LEFT JOIN Ingredients i ON pi.IngredientId = i.Id";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Dish> Dishes = new Dictionary<int, Dish>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetDishFromData(reader, Dishes);
                    }
                }
            }

            return Dishes.Values.ToList();
        }

        public async Task<List<Dish>> GetDishesByName(string name)
        {
            string query = @"SELECT p.*, c.Id AS CategoryId, c.Name AS CategoryName, i.Id AS IngredientId, i.Name AS IngredientName
                           FROM Pizzas p
                           LEFT JOIN Categories c ON p.CategoryId = c.Id
                           LEFT JOIN PizzaIngredient pi ON p.Id = pi.PizzaId
                           LEFT JOIN Ingredients i ON pi.IngredientId = i.Id
                           WHERE p.name LIKE @name";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Dish> Dishes = new Dictionary<int, Dish>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", $"%{name}%"));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetDishFromData(reader, Dishes);
                    }
                }
            }

            return Dishes.Values.ToList();
        }

        public async Task<Dish> GetDishById(int id)
        {
            string query = @"SELECT p.*, c.Id AS CategoryId, c.Name AS CategoryName, i.Id AS IngredientId, i.Name AS IngredientName
                           FROM Pizzas p
                           LEFT JOIN Categories c ON p.CategoryId = c.Id
                           LEFT JOIN PizzaIngredient pi ON p.Id = pi.PizzaId
                           LEFT JOIN Ingredients i ON pi.IngredientId = i.Id
                           WHERE p.id=@id";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Dish> Dish = new Dictionary<int, Dish>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetDishFromData(reader, Dish);
                    }
                }
            }

            return Dish.Values.FirstOrDefault();
        }

        public async Task<int> InsertDish(Dish Dish)
        {
            string query = $"INSERT INTO Dishes (Name, Description, Price, CategoryId) VALUES (@name, @description, @price, @categoryId)" +
                           $"SELECT SCOPE_IDENTITY();";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlTransaction transaction = (SqlTransaction)(await conn.BeginTransactionAsync()))
                {
                    try
                    {
                        cmd.Transaction = transaction;

                        cmd.Parameters.Add(new SqlParameter("@name", Dish.Name));
                        cmd.Parameters.Add(new SqlParameter("@description", Dish.Description ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@price", Dish.Price));
                        cmd.Parameters.Add(new SqlParameter("@categoryId", Dish.CategoryId ?? (object)DBNull.Value));

                        int dishId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        if (Dish.MenuIds.Any() || Dish.MenuIds != null)
                        {
                            await AddDishMenus(dishId, Dish.MenuIds, cmd);
                        }

                        await transaction.CommitAsync();

                        return dishId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> UpdateDish(int pizzaId, Dish Dish)
        {
            string query = $"UPDATE Dishes SET Name = @name, Description = @description, Price = @price, CategoryId = @categoryId WHERE id = @id";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlTransaction transaction = (SqlTransaction)(await conn.BeginTransactionAsync()))
                {
                    try
                    {
                        cmd.Transaction = transaction;

                        cmd.Parameters.Add(new SqlParameter("@id", pizzaId));
                        cmd.Parameters.Add(new SqlParameter("@name", Dish.Name));
                        cmd.Parameters.Add(new SqlParameter("@description", Dish.Description ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@price", Dish.Price));
                        cmd.Parameters.Add(new SqlParameter("@categoryId", Dish.CategoryId ?? (object)DBNull.Value));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (Dish.MenuIds.Any() || Dish.MenuIds != null)
                        {
                            await RemoveDishMenus(pizzaId, cmd);
                            await AddDishMenus(pizzaId, Dish.MenuIds, cmd);
                        }

                        await transaction.CommitAsync();

                        return rowsAffected;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> DeleteDish(int id)
        {
            string query = $"DELETE FROM Dishes WHERE id = @id";

            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlTransaction transaction = (SqlTransaction)(await conn.BeginTransactionAsync()))
                {
                    try
                    {
                        cmd.Transaction = transaction;

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();

                        return rowsAffected;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private async Task RemoveDishMenus(int dishId, SqlCommand cmd)
        {
            cmd.CommandText = $"DELETE FROM Menus_Dishes WHERE dish_id = @id";

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@id", dishId));

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task AddDishMenus(int dishId, List<int> MenuIds, SqlCommand cmd)
        {
            cmd.CommandText = $"INSERT INTO Menus_Dishes (menu_id, dish_id) VALUES (@MenuId, @DishId)";

            foreach (int menuId in MenuIds)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@MenuId", menuId));
                cmd.Parameters.Add(new SqlParameter("@DishId", dishId));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void GetDishFromData(SqlDataReader reader, Dictionary<int, Dish> Dishes)
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));

            if (!Dishes.TryGetValue(id, out Dish dish))
            {
                dish = new Dish();
                dish.Id = id;
                dish.Name = reader.GetString(reader.GetOrdinal("Name"));
                dish.Description = reader.GetString(reader.GetOrdinal("Description"));
                dish.Price = reader.GetDecimal(reader.GetOrdinal("Price"));

                if (!reader.IsDBNull(reader.GetOrdinal("CategoryId")))
                {
                    Category category = new Category();
                    category.Id = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                    category.Name = reader.GetString(reader.GetOrdinal("CategoryName"));

                    dish.CategoryId = category.Id;
                    dish.Category = category;
                }

                Dishes.Add(id, dish);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("MenuId")))
            {
                Menu menu = new Menu();
                menu.Id = reader.GetInt32(reader.GetOrdinal("IngredientId"));
                menu.Name = reader.GetString(reader.GetOrdinal("IngredientName"));

                dish.MenuIds.Add(menu.Id);
                dish.Menus.Add(menu);
            }
        }

        /*
        public async Task<int> OnCategoryDelete(int id)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            var query = $"UPDATE Pizzas SET CategoryId = NULL WHERE CategoryId = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));
                return await cmd.ExecuteNonQueryAsync();
            }
        }
        */
    }
}
