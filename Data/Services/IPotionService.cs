using HogwartsPotions.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Data.Services
{
    public interface IPotionService
    {
        Task<List<Potion>> GetAllPotions();
        Task<List<Potion>> GetAllPotionsByStudent(long studentId);
        Task AddPotion(Potion newPotion);
        Task<Student> GetStudent(long studentId);
        Task AddEmptyPotion(Potion newPotion);
    }
}
