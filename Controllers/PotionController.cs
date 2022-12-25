using System;
using HogwartsPotions.Data.Services;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/potions")]
    public class PotionController : Controller
    {
        private readonly IPotionService _service;
        private readonly int _maxIngredientsForPotions;

        public PotionController(IPotionService context)
        {
            _service = context;
            _maxIngredientsForPotions = 5;
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
            var studentPotions = await _service.GetAllPotionsOfStudent(studentId);
            if (studentPotions == null)
            {
                return NotFound();
            }

            if (studentPotions.Count == 0)
            {
                return NotFound($"Student of ID {studentId} doesn't have any potions yet!");
            }

            return Ok(studentPotions);
        }

        [HttpGet("student/{studentId:long}")]
        public async Task<ActionResult> GetStudentById(long studentId)
        {
            var student = await _service.GetStudent(studentId);
            if (student is null)
            {
                return NotFound($"Student with ID of {studentId} doesn't exist!");
            }

            return Ok(student);
        }

        [HttpPost("brew/{studentId:long}")]
        public async Task<ActionResult> BrewPotion(long studentId)
        {
            Student student = await _service.GetStudent(studentId);
            if (student is null)
            {
                return NotFound($"Student with ID of {studentId} doesn't exist!");
            }

            Potion potion = await _service.AddEmptyPotion(student);
            return Ok($"A new potion just brewed by {student.Name}");
        }

        [HttpGet("potion/{potionId:long}")]
        public async Task<IActionResult> GetPotionById(long potionId)
        {
            Potion potion = await _service.GetPotionById(potionId);
            return Ok(potion);
        }

        [HttpPut("{potionId:long}/add")]
        public async Task<ActionResult> AddNewIngredientToPotion(long potionId, [FromBody] Ingredient ingredient)
        {
            Potion potion = await _service.GetPotionById(potionId);
            if (potion is null)
            {
                return NotFound($"Potion with ID of {potionId} doesn't exist!");
            }

            if (potion.Ingredients.Count >= _maxIngredientsForPotions)
            {
                return NotFound($"Potion with ID of {potionId} has too many ingredients.");
            }

            List<Ingredient> ingredients = await _service.GetAllIngredients();

            foreach (Ingredient localIngredient in ingredients)
            {
                if (localIngredient.Name == ingredient.Name)
                {
                    await _service.AddNewIngredientToPotion(ingredient, potionId);

                    if (potion.Ingredients.Count >= _maxIngredientsForPotions)
                    {
                        await _service.ChangePotionStatus(potion);
                    }

                    return Ok(potion);
                }
            }
            await _service.AddIngredient(ingredient);
            await _service.AddNewIngredientToPotion(ingredient,potionId);

            if (potion.Ingredients.Count >= 5)
            {
                await _service.ChangePotionStatus(potion);
            }

            return Ok(potion);
        }

        [HttpGet("{potionId}/help")]
        public async Task<ActionResult<List<Recipe>>> GetAllRecipesByMatchingPotionIngredients(long potionId)
        {
            List<Recipe> recipes = await _service.GetAllRecipesByMatchingPotionIngredients(potionId);
            if (recipes.Count == 0)
            {
                return NotFound("No match found!");
            }
            return Ok(recipes);
        }
    }
}
