using HogwartsPotions.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Data.Services
{
    public interface IPotionService
    {
        Task<List<Potion>> GetAllPotions();
        Task<List<Potion>> GetAllPotionsOfStudent(long studentId);
        Task<Potion> AddPotion(Potion potion);
        Task<Student> GetStudent(long studentId);
        Task<Potion> AddEmptyPotion(Student student);
        Task<Potion> GetPotionById(long potionId);
        Task<Potion> AddNewIngredientToPotion(Ingredient ingredient,Potion potion);
        Task<List<Ingredient>> GetAllIngredients();
        Task<Ingredient> GetIngredientByName(string name);
        Task<Potion> ChangePotionStatus(Potion potion);
        Task<List<Recipe>> GetAllRecipes();
        Task AddIngredient(Ingredient ingredient);
        Task<List<Recipe>> GetAllRecipesByMatchingPotionIngredients(long potionId);
        bool IsIngredientInPotion(Potion potion, Ingredient ingredient);
        bool IsPotionIngredientsFull(Potion potion);
    }
}
