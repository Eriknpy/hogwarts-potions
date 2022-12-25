using HogwartsPotions.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Data.Services
{
    public interface IPotionService
    {
        Task<List<Potion>> GetAllPotions();
        Task<List<Potion>> GetAllPotionsOfStudent(long studentId);
        Task AddPotion(Potion potion);
        Task<Student> GetStudent(long studentId);
        Task<Potion> AddEmptyPotion(Student student);
        Task<Potion> GetPotionById(long potionId);
        Task<Potion> AddNewIngredientToPotion(Ingredient ingredient,long potionId);
        Task<List<Ingredient>> GetAllIngredients();
        Task<Ingredient> GetIngredientByName(string name);
        Task ChangePotionStatus(Potion potion);
        Task<List<Recipe>> GetAllRecipes();
        Task AddIngredient(Ingredient ingredient);
        Task<List<Recipe>> GetAllRecipesByMatchingPotionIngredients(long potionId);
    }
}
