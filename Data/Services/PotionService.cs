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

        public async Task<List<Potion>> GetAllPotionsOfStudent(long studentId)
        {
            return await _context.Potions
                .Where(potion => potion.Brewer.Id == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Potion> AddPotion(Potion potion)
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
                    return newPotion;
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
                    return newPotion;
                }
            }
            return null;
        }

        public async Task<Student> GetStudent(long studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(student => student.Id == studentId);
        }

        public async Task<Potion> AddEmptyPotion(Student student)
        {
            Potion emptyPotion = new Potion()
            {
                Status = BrewingStatus.Brew,
                Brewer = student
            };
            await _context.Potions.AddAsync(emptyPotion);
            await _context.SaveChangesAsync();
            return emptyPotion;
        }

        public async Task<Potion> GetPotionById(long potionId)
        {
            return await _context.Potions.Where(potion => potion.Id == potionId)
                .Include(p=>p.Brewer)
                .Include(p => p.Ingredients)
                .Include(p=>p.Recipe)
                .FirstOrDefaultAsync();
        }

        public async Task<Potion> AddNewIngredientToPotion(Ingredient ingredient, Potion potion)
        {
            if (isIngredientInDatabase(ingredient))
            {
                if (!IsIngredientInPotion(potion, ingredient))
                {
                    var existingIngredient = await GetIngredientByName(ingredient.Name);
                    await UpdatePotionIngredients(potion, existingIngredient);
                    if (IsPotionIngredientsFull(potion))
                    {
                        await ChangePotionStatus(potion);
                    }
                    return potion;
                }
            }
            await AddIngredient(ingredient);
            var newIngredient = await GetIngredientByName(ingredient.Name);
            await UpdatePotionIngredients(potion, newIngredient);
            if (IsPotionIngredientsFull(potion))
            {
                await ChangePotionStatus(potion);
            }
            return potion;
        }

        public async Task UpdatePotionIngredients(Potion potion,Ingredient ingredient)
        {
            potion.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        public bool IsIngredientInPotion(Potion updatePotion, Ingredient ingredient)
        {
            return updatePotion.Ingredients.Any(potionIngredient => potionIngredient.Name == ingredient.Name);
        }

        public bool IsPotionIngredientsFull(Potion potion)
        {
      
            return potion.Ingredients.Count >= MaxIngredientsForPotions;
        }

        private bool isIngredientInDatabase(Ingredient ingredient)
        {
            return _context.Ingredients.Any(dbIngredient => dbIngredient.Name == ingredient.Name);
        }

        public async Task<List<Ingredient>> GetAllIngredients()
        {
            return await _context.Ingredients.AsNoTracking().ToListAsync();
        }

        public async Task<Ingredient> GetIngredientByName(string name)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == name);
        }

        public async Task<Potion> ChangePotionStatus(Potion potion)
        {
            var recipes = await GetAllRecipes();

            foreach (var recipe in recipes)
            {
                if (IsIngredientsMatch(potion.Ingredients))
                {
                    potion.Status = BrewingStatus.Replica;
                    potion.Recipe = recipe;
                    potion.Name = $"{potion.Brewer.Name}'s replica #{GetReplicaCountByPotionBrewer(potion)}";
                    await _context.SaveChangesAsync();
                    return potion;
                }
            }
            potion.Status = BrewingStatus.Discovery;
            var newRecipe = new Recipe()
            {
                Name = $"{potion.Brewer.Name}'s discovery #{GetDiscoveryCountByPotionBrewer(potion)}",
                Ingredients = potion.Ingredients,
                Author = potion.Brewer
            };
            await _context.Recipes.AddAsync(newRecipe);
            potion.Recipe = newRecipe;
            potion.Name = potion.Recipe.Name;
            await _context.SaveChangesAsync();
            return potion;
        }

        public int GetReplicaCountByPotionBrewer(Potion potion)
        {
            int baseIndex = 1;
            return _context.Potions.Count(p =>
                p.Brewer == potion.Brewer &&
                p.Status == BrewingStatus.Replica) + baseIndex;
        }

        public int GetDiscoveryCountByPotionBrewer(Potion potion)
        {
            int baseIndex = 1;
            return  _context.Potions.Count(p =>
                p.Status == BrewingStatus.Discovery && p.Brewer == potion.Brewer) + baseIndex;
        }
        public async Task<List<Recipe>> GetAllRecipes()
        {
            return await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.Ingredients)
                .ToListAsync();
        }

        public async Task AddIngredient(Ingredient ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Recipe>> GetAllRecipesByMatchingPotionIngredients(long potionId)
        {
            Potion potion = await GetPotionById(potionId);

            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.Ingredients.Any(ingredient => potion.Ingredients.Contains(ingredient))).AsNoTracking().ToListAsync();
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
