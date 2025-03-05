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
            string query = @"SELECT d.*, c.Id AS CategoryId, c.Name AS CategoryName
                             FROM Dishes d
                             LEFT JOIN Categories c ON d.category_id = c.Id;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            List<Dish> dishes = new List<Dish>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dishes.Add(GetDishFromData(reader));
                    }
                }
            }

            return dishes;
        }

        public async Task<List<Dish>> GetDishesByName(string name)
        {
            string query = @"SELECT d.*, c.Id AS CategoryId, c.Name AS CategoryName
                             FROM Dishes d
                             LEFT JOIN Categories c ON d.category_id = c.Id
                             WHERE d.name LIKE @name;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            List<Dish> dishes = new List<Dish>(); ;

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", $"%{name}%"));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dishes.Add(GetDishFromData(reader));
                    }
                }
            }

            return dishes;
        }

        public async Task<Dish> GetDishById(int id)
        {
            string query = @"SELECT d.*, c.Id AS CategoryId, c.Name AS CategoryName
                             FROM Dishes d
                             LEFT JOIN Categories c ON d.category_id = c.Id
                             WHERE d.id=@id;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return GetDishFromData(reader);
                    }
                }
            }

            return null;
        }

        public async Task<int> InsertDish(Dish Dish)
        {
            string query = @"INSERT INTO Dishes (Name, Description, Price, CategoryId)
                             VALUES (@name, @description, @price, @categoryId)
                             SELECT SCOPE_IDENTITY();";

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
            string query = @"UPDATE Dishes SET Name = @name, Description = @description, Price = @price, CategoryId = @categoryId
                             WHERE id=@id;";

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

                        int affectedRows = await cmd.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();

                        return affectedRows;
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
            string query = @"DELETE FROM Dishes
                             WHERE id = @id;";

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

                        int affectedRows = await cmd.ExecuteNonQueryAsync();

                        await transaction.CommitAsync();

                        return affectedRows;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private Dish GetDishFromData(SqlDataReader reader)
        {
            Dish dish = new Dish();
            dish.Id = reader.GetInt32(reader.GetOrdinal("id"));
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

            return dish;
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
