using System;
using HogwartsPotions.Data.Services;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/potions")]
    public class PotionController : Controller
    {
        private readonly IPotionService _service;
        public PotionController(IPotionService context)
        {
            _service = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Potion>>> GetAllPotions()
        {
            var potions = await _service.GetAllPotions();
            if (potions is null)
            {
                return NotFound();
            }
            return Ok(potions);
        }
    }
}
