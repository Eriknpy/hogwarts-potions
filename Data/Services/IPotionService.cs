using HogwartsPotions.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Data.Services
{
    public interface IPotionService
    {
        Task<List<Potion>> GetAllPotions();
    }
}
