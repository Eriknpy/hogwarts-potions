using HogwartsPotions.Data.Services;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/potion")]
    public class PotionController : Controller
    {
        private readonly IPotionService _service;
        public PotionController(IPotionService context)
        {
            _service = context;
        }

        [HttpGet("all")]
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _service.GetAllPotions();
        }
    }
}
