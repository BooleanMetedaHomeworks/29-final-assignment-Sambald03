using Microsoft.Data.SqlClient;
using ristorante_backend.Models;
using static ristorante_backend.DB;

namespace ristorante_backend.Repositories
{
    public class CategoryRepository
    {
        public async Task<List<Category>> GetAllCategories()
        {
            string query = @"SELECT *
                             FROM Categories;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            List<Category> categories = new List<Category>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(GetCategoryFromData(reader));
                    }
                }
            }

            return categories;
        }

        public async Task<List<Category>> GetCategoriesByName(string name)
        {
            string query = @"SELECT *
                             FROM Categories
                             WHERE name LIKE @name;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            List<Category> categories = new List<Category>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", $"%{name}%"));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(GetCategoryFromData(reader));
                    }
                }
            }

            return categories;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            string query = @"SELECT *
                             FROM Categories
                             WHERE id=@id;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return GetCategoryFromData(reader);
                    }
                }
            }

            return null;
        }

        public async Task<int> InsertCategory(Category category)
        {
            string query = @"INSERT INTO Categories (Name)
                             VALUES (@name)
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

                        cmd.Parameters.Add(new SqlParameter("@name", category.Name));

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

        public async Task<int> UpdateCategory(int pizzaId, Category category)
        {
            string query = @"UPDATE Categories SET Name = @name
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
                        cmd.Parameters.Add(new SqlParameter("@name", category.Name));

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

        public async Task<int> DeleteCategory(int id)
        {
            string query = @"DELETE FROM Categories
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

        private Category GetCategoryFromData(SqlDataReader reader)
        {
            Category category = new Category();
            category.Id = reader.GetInt32(reader.GetOrdinal("id"));
            category.Name = reader.GetString(reader.GetOrdinal("Name"));

            return category;
        }
    }
}
