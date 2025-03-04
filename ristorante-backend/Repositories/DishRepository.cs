﻿using Azure;
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
            string query = @"SELECT p.*, c.Id AS CategoryId, c.Name AS CategoryName,
                           i.Id AS IngredientId, i.Name AS IngredientName
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
            string query = $"INSERT INTO Pizzas (Name, Description, Price, CategoryId) VALUES (@name, @description, @price, @categoryId)" +
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
                            await AddDishMenus(dishId, Dish.MenuIds, conn);
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
            var query = $"UPDATE Pizzas SET Name = @name, Description = @description, Price = @price, CategoryId = @categoryId WHERE id = @id";

            using var conn = new SqlConnection(CONNECTION_STRING);
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
                            await RemoveDishMenus(pizzaId, conn);
                            await AddDishMenus(pizzaId, Dish.MenuIds, conn);
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
            var query = $"DELETE FROM Pizzas WHERE id = @id";

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

        private async Task RemoveDishMenus(int DishId, SqlConnection conn)
        {
            var query = $"DELETE FROM PizzaIngredient WHERE PizzaId = @id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", DishId));
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task AddDishMenus(int DishId, List<int> Ingredients, SqlConnection conn)
        {
            var insertIngredientQuery = $"INSERT INTO PizzaIngredient (PizzaId, IngredientId) VALUES (@PizzaId, @IngredientId)";

            foreach (int IngredientId in Ingredients)
            {
                using (SqlCommand cmd = new SqlCommand(insertIngredientQuery, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@PizzaId", PizzaId));
                    cmd.Parameters.Add(new SqlParameter("@IngredientId", IngredientId));
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private void GetPizzaFromData(SqlDataReader reader, Dictionary<int, Pizza> pizzas)
        {
            var id = reader.GetInt32(reader.GetOrdinal("id"));
            if (pizzas.TryGetValue(id, out Pizza pizza) == false)
            {
                var name = reader.GetString(reader.GetOrdinal("Name"));
                var description = reader.GetString(reader.GetOrdinal("Description"));
                var price = reader.GetDecimal(reader.GetOrdinal("Price"));
                pizza = new Pizza(id, name, description, price);
                pizzas.Add(id, pizza);
            }
            if (reader.IsDBNull(reader.GetOrdinal("CategoryId")) == false)
            {
                var categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                var categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                Category c = new Category();
                c.Id = categoryId;
                c.Name = categoryName;
                pizza.Category = c;
                pizza.CategoryId = categoryId;
            }
            if (reader.IsDBNull(reader.GetOrdinal("IngredientId")) == false)
            {
                var ingredientId = reader.GetInt32(reader.GetOrdinal("IngredientId"));
                var ingredientName = reader.GetString(reader.GetOrdinal("IngredientName"));
                Ingredient i = new Ingredient(ingredientId, ingredientName);
                pizza.IngredientIds.Add(ingredientId);
                pizza.Ingredients.Add(i);
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
