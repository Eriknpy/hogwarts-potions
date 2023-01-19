using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Data.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("[controller]")]
    public class PotionController : Controller
    {
        private readonly IPotionService _service;

        public PotionController(IPotionService context)
        {
            _service = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPotions()
        {
            var potions = await _service.GetAllPotions();
            if (potions != null)
            {
                return StatusCode(StatusCodes.Status200OK, potions);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"No potions found!");
        }

        [HttpGet("{studentId:long}")]
        public async Task<IActionResult> GetAllPotionsByStudent(long studentId)
        {
            Student student = await _service.GetStudent(studentId);
            if (student != null)
            {
                var studentPotions = await _service.GetAllPotionsOfStudent(student);
                if (studentPotions.Count == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Student of Id {studentId} doesn't have any potions yet!");
                }
                return StatusCode(StatusCodes.Status200OK, studentPotions);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Student with Id of {studentId} doesn't exist!");
        } 

        [HttpPost]
        public async Task<IActionResult> AddPotion([FromBody] Potion potion)
        {
            Student brewer = await _service.GetStudent(potion.Brewer.Id);
            if (brewer != null)
            {
                var addPotion = await _service.AddPotionByStatus(potion, brewer);
                if (addPotion != null)
                {
                    return StatusCode(StatusCodes.Status201Created, addPotion);
                }
                return StatusCode(StatusCodes.Status404NotFound, $@"At this endpoint only Discovery or Replica potion can be added. Your potion has {potion.Ingredients.Count} ingredients. It must be 5 !
If you wish to brew a new potion, please visit-> potion/brew/studentId to brew an empty potion.
Then you can add Ingredients, please visit -> /potion/potionId/add");
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Student with Id of {potion.Brewer.Id} doesn't exist!");
        }


        [HttpGet("student/{studentId:long}")]
        public async Task<IActionResult> GetStudentById(long studentId)
        {
            var student = await _service.GetStudent(studentId);
            if (student != null)
            {
                return StatusCode(StatusCodes.Status200OK, student);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Student with Id of {studentId} doesn't exist!");
        }

        [HttpPost("brew/{studentId:long}")]
        public async Task<IActionResult> BrewPotion(long studentId)
        {
            Student student = await _service.GetStudent(studentId);
            if (student != null)
            {
                await _service.AddEmptyPotion(student);
                return StatusCode(StatusCodes.Status201Created, $"A new potion just brewed by {student.Name}");
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Student with Id of {studentId} doesn't exist!");
        }

        [HttpGet("potion/{potionId:long}")]
        public async Task<IActionResult> GetPotionById(long potionId)
        {
            Potion potion = await _service.GetPotionById(potionId);
            if (potion != null)
            {
                return StatusCode(StatusCodes.Status200OK, potion);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"No potions found!");
        }

        [HttpPut("{potionId:long}/add")]
        public async Task<IActionResult> AddNewIngredientToPotion(long potionId, [FromBody] Ingredient ingredient)
        {
            Potion potion = await _service.GetPotionById(potionId);
            if (potion != null)
            {
                if (_service.IsIngredientInPotion(potion, ingredient))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"{ingredient.Name} already exists in the current potion");
                }
                if (_service.IsPotionIngredientsFull(potion))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Potion with Id of {potion.Id} has too many ingredient.");
                }
                await _service.AddNewIngredientToPotion(ingredient, potion);
                return StatusCode(StatusCodes.Status200OK, potion);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Potion with Id of {potionId} doesn't exist!");
        }

        [HttpGet("{potionId}/help")]
        public async Task<ActionResult<List<Recipe>>> GetAllRecipesByMatchingPotionIngredients(long potionId)
        {
            var potion = await _service.GetPotionById(potionId);
            if (!_service.IsPotionIngredientsFull(potion))
            {
                var recipes = await _service.GetAllRecipesByMatchingPotionIngredients(potion);
                if (recipes.Count == 0)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "No match found!");
                }
                return StatusCode(StatusCodes.Status200OK, recipes);
            }
            return StatusCode(StatusCodes.Status400BadRequest, $"The current potion already brewed");
            
        }
    }
}