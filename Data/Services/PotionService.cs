using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Data.Services
{
    public class PotionService : IPotionService
    {
        private readonly HogwartsContext _context;

        public PotionService(HogwartsContext context)
        {
            _context = context;
        }
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _context.Potions
                .Include(p=>p.Ingredients)
                .ToListAsync();
        }
    }
}
