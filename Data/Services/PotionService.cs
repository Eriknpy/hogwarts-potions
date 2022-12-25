using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data.Enums;
using HogwartsPotions.Models.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HogwartsPotions.Data.Services
{
    public class PotionService : IPotionService
    {
        private readonly HogwartsContext _context;
        public const int MaxIngredientsForPotions = 5;

        public PotionService(HogwartsContext context)
        {
            _context = context;
        }
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _context.Potions
                .Include(p => p.Brewer)
                .Include(p => p.Ingredients)
                .Include(p => p.Recipe)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Potion>> GetAllPotionsByStudent(long studentId)
        {
            return await _context.Potions
                .Where(potion => potion.Brewer.Id == studentId)
                .Include(potion => potion.Ingredients)
                .Include(potion => potion.Brewer)
                .Include(potion => potion.Recipe)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddPotion(Potion potion)
        {
            Student student = await GetStudent(potion.Brewer.Id);
            Potion newPotion = new Potion();
            potion.Brewer = student;
            int baseIndex = 1;
            if (potion.Ingredients.Count == MaxIngredientsForPotions)
            {
                if (IsIngredientsMatch(potion.Ingredients))
                {
                    newPotion.Ingredients = GetRecipeByIngredients(potion.Ingredients).Ingredients;
                    newPotion.Status = BrewingStatus.Replica;
                    newPotion.Recipe = GetRecipeByIngredients(potion.Ingredients);
                    newPotion.Brewer = potion.Brewer;
                    int count = _context.Potions.Count(p =>
                        p.Brewer == newPotion.Brewer &&
                        p.Status == BrewingStatus.Replica) + baseIndex;

                    newPotion.Name = $"{student.Name}'s replica #{count}";

                    await _context.Potions.AddAsync(newPotion);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    foreach (var ingredient in potion.Ingredients)
                    {
                        if (!_context.Ingredients.Any(i => i.Name == ingredient.Name))
                        {
                            await _context.Ingredients.AddAsync(ingredient);
                            newPotion.Ingredients.Add(ingredient);
                        }
                        else
                        {
                            var existingIngredient = await _context.Ingredients.Where(i => i.Name == ingredient.Name)
                                .FirstAsync();
                            newPotion.Ingredients.Add(existingIngredient);
                        }
                    }
                    newPotion.Status = BrewingStatus.Discovery;
                    newPotion.Brewer = potion.Brewer;
                    int count = _context.Potions.Count(p =>
                        p.Status == BrewingStatus.Discovery && p.Brewer == newPotion.Brewer) + baseIndex;
                    newPotion.Recipe = new Recipe()
                    {
                        Name = $"{newPotion.Brewer.Name}'s discovery #{count}",
                        Ingredients = newPotion.Ingredients,
                        Author = newPotion.Brewer
                    };
                    newPotion.Name = newPotion.Recipe.Name;
                    await _context.Potions.AddAsync(newPotion);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception("Only Discovery or Replica potions allowed.Therefore, If you wish to brew a new potion with less than 5 ingredients,please visit-> ");
            }
        }

        public async Task<Student> GetStudent(long studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(student => student.Id == studentId);
        }

        public Task AddEmptyPotion(Potion newPotion)
        {
            throw new NotImplementedException();
        }

        private Recipe GetRecipeByIngredients(HashSet<Ingredient> potionIngredients)
        {
            return _context.Recipes
                .Include(recipe => recipe.Ingredients)
                .AsEnumerable()
                .FirstOrDefault(recipe => recipe.Ingredients
                    .Select(ingredient => ingredient.Name)
                    .OrderBy(name => name)
                    .SequenceEqual(potionIngredients
                        .Select(ingredient => ingredient.Name)
                        .OrderBy(name => name)));
        }

        private bool IsIngredientsMatch(HashSet<Ingredient> newPotionIngredients)
        {
            //List<string> PotionIngredients = new List<string>();
            //foreach (var ingredient in newPotionIngredients)
            //{
            //    PotionIngredients.Add(ingredient.Name);
            //}

            //var Recipes = _context.Recipes;
            //foreach (var recipe in Recipes)
            //{
            //    List<string> RecipeIngredients = new List<string>();
            //    foreach (var ingredient in recipe.Ingredients) // Object reference null
            //    {
            //        RecipeIngredients.Add(ingredient.Name);
            //    }
            //    bool isEqual = !RecipeIngredients.Except(PotionIngredients).Any();
            //    if (isEqual)
            //    {
            //        return true;
            //    }
            //}
            //return false;
            return _context.Recipes
                .Include(recipe => recipe.Ingredients)
                .AsEnumerable()
                .Any(recipe => recipe.Ingredients
                    .Select(ingredient => ingredient.Name)
                    .OrderBy(name => name)
                    .SequenceEqual(newPotionIngredients
                        .Select(ingredient => ingredient.Name)
                        .OrderBy(name => name)));
        }
    }
}
