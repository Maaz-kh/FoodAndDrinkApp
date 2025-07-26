using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodAndDrinkApp.Models;

namespace FoodAndDrinkApp.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;    // Singleton instance of the database connection

        public static async Task InitAsync()      // Checks if the database is initialized, and initializes it if not
        {
            if (_database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "recipes.db");  // Path to the database file
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Recipe>();    // Create the Recipe table if it doesn't exist
        }


        public static async Task<List<Recipe>> GetRecipesAsync()
        {
            await InitAsync();     // Ensure the database is initialized
            return await _database.Table<Recipe>().ToListAsync();   // Fetches all rows from the Recipe table and returns them as a List<Recipe>.
        }

        public static async Task AddRecipeAsync(Recipe recipe)
        {
            await InitAsync();
            await _database.InsertAsync(recipe);  // Inserts a new recipe into the Recipe table.
        }

        public static async Task DeleteRecipeAsync(Recipe recipe)
        {
            await InitAsync();
            await _database.DeleteAsync(recipe);    // Deletes a recipe from the Recipe table.
        }

        public static async Task UpdateRecipeAsync(Recipe recipe)
        {
            var existing = await _database.Table<Recipe>().Where(r => r.Id == recipe.Id).FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Title = recipe.Title;
                existing.Instructions = recipe.Instructions;
                existing.Ingredients = recipe.Ingredients;
                existing.EstimatedTime = recipe.EstimatedTime;
                existing.ImagePath = recipe.ImagePath;
                await _database.UpdateAsync(existing);
            }
        }
    }
}