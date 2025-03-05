using Microsoft.Data.SqlClient;
using ristorante_backend.Models;
using static ristorante_backend.DB;

namespace ristorante_backend.Repositories
{
    public class MenuRepository
    {
        public async Task<List<Menu>> GetAllMenus()
        {
            string query = @"SELECT m.*, d.id AS DishId, d.name AS DishName, d.description AS DishDescription, d.price AS DishPrice, c.id AS CategoryId, c.name AS CategoryName
                             FROM Menus m
                             LEFT JOIN Menus_Dishes md ON m.Id = md.menu_id
                             LEFT JOIN Dishes d ON md.dish_id = d.Id
                             LEFT JOIN Categories c ON d.category_id = c.id;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Menu> menus = new Dictionary<int, Menu>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetMenuFromData(reader, menus);
                    }
                }
            }

            return menus.Values.ToList();
        }

        public async Task<List<Menu>> GetMenusByName(string name)
        {
            string query = @"SELECT m.*, d.id AS DishId, d.name AS DishName, d.description AS DishDescription, d.price AS DishPrice, c.id AS CategoryId, c.name AS CategoryName
                             FROM Menus m
                             LEFT JOIN Menus_Dishes md ON m.Id = md.menu_id
                             LEFT JOIN Dishes d ON md.dish_id = d.Id
                             LEFT JOIN Categories c ON d.category_id = c.id
                             WHERE m.name LIKE @name;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Menu> menus = new Dictionary<int, Menu>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", $"%{name}%"));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetMenuFromData(reader, menus);
                    }
                }
            }

            return menus.Values.ToList();
        }

        public async Task<Menu> GetMenuById(int id)
        {
            string query = @"SELECT m.*, d.id AS DishId, d.name AS DishName, d.description AS DishDescription, d.price AS DishPrice, c.id AS CategoryId, c.name AS CategoryName
                             FROM Menus m
                             LEFT JOIN Menus_Dishes md ON m.Id = md.menu_id
                             LEFT JOIN Dishes d ON md.dish_id = d.Id
                             LEFT JOIN Categories c ON d.category_id = c.id
                             WHERE p.id=@id;";

            using SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            Dictionary<int, Menu> menus = new Dictionary<int, Menu>();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        GetMenuFromData(reader, menus);
                    }
                }
            }

            return menus.Values.FirstOrDefault();
        }

        public async Task<int> InsertMenu(Menu menu)
        {
            string query = @"INSERT INTO Menus (Name)
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

                        cmd.Parameters.Add(new SqlParameter("@name", menu.Name));

                        int menuId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        if (menu.DishIds?.Any() == true)
                        {
                            await AddMenuDishes(menuId, menu.DishIds, cmd);
                        }

                        await transaction.CommitAsync();

                        return menuId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> UpdateMenu(int menuId, Menu menu)
        {
            string query = @"UPDATE Menus SET Name = @name
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

                        cmd.Parameters.Add(new SqlParameter("@id", menuId));
                        cmd.Parameters.Add(new SqlParameter("@name", menu.Name));

                        int affectedRows = await cmd.ExecuteNonQueryAsync();

                        if (menu.DishIds?.Any() == true)
                        {
                            await RemoveMenuDishes(menuId, cmd);
                            await AddMenuDishes(menuId, menu.DishIds, cmd);
                        }

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

        public async Task<int> DeleteMenu(int id)
        {
            string query = @"DELETE FROM Menus
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

        private async Task RemoveMenuDishes(int menuId, SqlCommand cmd)
        {
            cmd.CommandText = @"DELETE FROM Menus_Dishes
                                WHERE menu_id = @id;";

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@id", menuId));

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task AddMenuDishes(int menuId, List<int> dishIds, SqlCommand cmd)
        {
            cmd.CommandText = @"INSERT INTO Menus_Dishes (menu_id, dish_id)
                                VALUES (@MenuId, @DishId);";

            foreach (int dishId in dishIds)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@MenuId", menuId));
                cmd.Parameters.Add(new SqlParameter("@DishId", dishId));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void GetMenuFromData(SqlDataReader reader, Dictionary<int, Menu> menus)
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));

            if (!menus.TryGetValue(id, out Menu menu))
            {
                menu = new Menu();
                menu.Id = id;
                menu.Name = reader.GetString(reader.GetOrdinal("Name"));

                menus.Add(id, menu);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("DishId")))
            {
                Dish dish = new Dish();
                dish.Id = reader.GetInt32(reader.GetOrdinal("DishId"));
                dish.Name = reader.GetString(reader.GetOrdinal("DishName"));
                dish.Description = reader.GetString(reader.GetOrdinal("DishDescription"));
                dish.Price = reader.GetDecimal(reader.GetOrdinal("DishPrice"));

                if (!reader.IsDBNull(reader.GetOrdinal("CategoryId")))
                {
                    Category category = new Category();
                    category.Id = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                    category.Name = reader.GetString(reader.GetOrdinal("CategoryName"));

                    dish.CategoryId = category.Id;
                    dish.Category = category;
                }

                menu.DishIds.Add(dish.Id);
                menu.Dishes.Add(dish);
            }
        }
    }
}
