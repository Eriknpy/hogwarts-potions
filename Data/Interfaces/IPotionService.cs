using HogwartsPotions.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Data.Interfaces
{
    public interface IPotionService
    {
        Task<List<Potion>> GetAllPotions();
        Task<List<Potion>> GetAllPotionsOfStudent(Student student);
        Task<Potion> AddPotionByStatus(Potion potion, Student brewer);
        Task<Student> GetStudent(long studentId);
        Task<Potion> AddEmptyPotion(Student student);
        Task<Potion> GetPotionById(long potionId);
        Task<Potion> AddNewIngredientToPotion(Ingredient ingredient, Potion potion);
        Task<List<Recipe>> GetAllRecipesByMatchingPotionIngredients(Potion potion);
        bool IsIngredientInPotion(Potion potion, Ingredient ingredient);
        bool IsPotionIngredientsFull(Potion potion);
    }
}