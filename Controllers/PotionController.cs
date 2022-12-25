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

        [HttpPost]
        public async Task<ActionResult> AddPotion([FromBody] Potion potion)
        {
            Student student = await _service.GetStudent(potion.Brewer.Id);
            if (student is null)
            {
                return NotFound($"Student with ID of {potion.Brewer.Id} doesn't exist!");
            }

            await _service.AddPotion(potion);
            return Created("New Potion Added", potion);
        }

        [HttpGet("{studentId:long}")]
        public async Task<ActionResult> GetAllPotionsByStudent(long studentId)
        {
            var studentPotions = await _service.GetAllPotionsByStudent(studentId);
            if (studentPotions == null)
            {
                return NotFound();
            }

            if (studentPotions.Count == 0)
            {
                return NotFound($"{studentId}. Student doesn't have any potions yet!");
            }
            return Ok(studentPotions);
        }

        [HttpGet("student/{studentId:long}")]
        public async Task<ActionResult> GetStudentById(long studentId)
        {
            var student = await _service.GetStudent(studentId);
            if (student is null)
            {
                return NotFound($"Student #{studentId} doesn't exist!");
            }
            return Ok(student);
        }

        [HttpPost("brew/{studentId:long}")]
        public async Task<ActionResult> BrewPotion()
        {
            throw new NotImplementedException();
        }
    }
}
