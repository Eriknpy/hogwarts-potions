using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data.Enums;
using HogwartsPotions.Data.Interfaces;
using HogwartsPotions.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        #region Endpoints
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _context.Potions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Potion>> GetAllPotionsOfStudent(Student student)
        {
            return await _context.Potions
                .Where(potion => potion.Brewer.Id == student.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Potion> AddPotionByStatus(Potion potion, Student brewer)
        {
            if (potion.Ingredients.Count == MaxIngredientsForPotions)
            {
                if (IsIngredientsMatch(potion.Ingredients))
                {
                    var replicaPotion = CreateNewPotion(potion, brewer, true);
                    await AddPotion(replicaPotion);
                    return replicaPotion;
                }
                var discoveryIngredients = await IngredientsCheckerAsync(potion);
                var discoveryPotion = CreateNewPotion(discoveryIngredients, brewer, false);
                await AddPotion(discoveryPotion);
                return discoveryPotion;
            }
            return null;
        }

        public async Task<Student> GetStudent(long studentId)
        {
            return await _context.Students.FindAsync(studentId);
        }

        public async Task<Potion> AddEmptyPotion(Student student)
        {
            Potion emptyPotion = new Potion();
            emptyPotion.Status = BrewingStatus.Brew;
            emptyPotion.Brewer = student;
            emptyPotion.Name = $"{student.Name}'s empty potion #{PotionCounter(emptyPotion)}";
            await AddPotion(emptyPotion);
            return emptyPotion;
        }

        public async Task<Potion> GetPotionById(long potionId)
        {
            return await _context.Potions.Where(potion => potion.Id == potionId)
                .Include(p => p.Brewer)
                .Include(p => p.Ingredients)
                .Include(p => p.Recipe)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Potion> AddNewIngredientToPotion(Ingredient newIngredient, Potion potion)
        {
            if (IsIngredientExist(newIngredient))
            {
                var ingredient = await GetIngredientByNameAsync(newIngredient);
                if (!IsIngredientInPotion(potion, ingredient))
                {
                    potion.Ingredients.Add(ingredient);
                    await UpdatePotion(potion);
                    if (IsPotionIngredientsFull(potion))
                    {
                        await ChangePotionStatus(potion);
                    }
                    return potion;
                }
            }
            await AddIngredient(newIngredient);
            potion.Ingredients.Add(newIngredient);
            await UpdatePotion(potion);
            if (IsPotionIngredientsFull(potion))
            {
                await ChangePotionStatus(potion);
            }
            return potion;
        }

        public async Task<List<Recipe>> GetAllRecipesByMatchingPotionIngredients(Potion potion)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.Ingredients.Any(i => potion.Ingredients.Contains(i)))
                .ToListAsync();
        }
        #endregion

        #region Helper methods
        private async Task<Potion> IngredientsCheckerAsync(Potion potion)
        {
            Potion newPotion = new Potion();
            foreach (var ingredient in potion.Ingredients)
            {
                if (!IsIngredientExist(ingredient))
                {
                    await AddIngredient(ingredient);
                    newPotion.Ingredients.Add(ingredient);
                }
                else
                {
                    var existingIngredient = await GetIngredientByNameAsync(ingredient);
                    newPotion.Ingredients.Add(existingIngredient);
                }
            }
            return newPotion;
        }

        public async Task AddIngredient(Ingredient ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task AddPotion(Potion potion)
        {
            await _context.Potions.AddAsync(potion);
            await _context.SaveChangesAsync();
        }

        private async Task<Ingredient> GetIngredientByNameAsync(Ingredient ingredient)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredient.Name);
        }

        public Potion CreateNewPotion(Potion potion, Student brewer, bool replica)
        {
            var newPotion = new Potion();
            newPotion.Brewer = brewer;
            newPotion.Status = replica ? BrewingStatus.Replica : BrewingStatus.Discovery;
            newPotion.Ingredients = replica ? GetRecipeByIngredients(potion.Ingredients).Ingredients : potion.Ingredients;
            newPotion.Recipe = replica ? GetRecipeByIngredients(potion.Ingredients) : CreateRecipeByPotion(newPotion);
            newPotion.Name = $"{brewer.Name}'s {newPotion.Status} #{PotionCounter(newPotion)}";
            return newPotion;
        }

        private Recipe CreateRecipeByPotion(Potion potion)
        {
            return new Recipe()
            {
                Name = $"{potion.Brewer.Name}'s discovery #{PotionCounter(potion)}",
                Ingredients = potion.Ingredients,
                Author = potion.Brewer
            };
        }

        private int PotionCounter(Potion potion)
        {
            var baseIndex = 1;
            var count = _context.Potions.Count(p =>
                p.Brewer == potion.Brewer &&
                p.Status == potion.Status) + baseIndex;
            return count;
        }

        public async Task UpdatePotion(Potion potion)
        {
            EntityEntry entityEntryPotion = _context.Entry(potion);
            entityEntryPotion.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool IsIngredientInPotion(Potion potion, Ingredient ingredient)
        {
            return potion.Ingredients.Any(potionIngredient => potionIngredient.Name == ingredient.Name);
        }

        public bool IsPotionIngredientsFull(Potion potion)
        {

            return potion.Ingredients.Count >= MaxIngredientsForPotions;
        }

        private bool IsIngredientExist(Ingredient ingredient)
        {
            return _context.Ingredients.Any(i => i.Name == ingredient.Name);
        }

        public async Task<Potion> ChangePotionStatus(Potion potion)
        {
            if (IsIngredientsMatch(potion.Ingredients))
            {
                potion.Status = BrewingStatus.Replica;
                potion.Recipe = GetRecipeByIngredients(potion.Ingredients);
                potion.Name = $"{potion.Brewer.Name}'s {potion.Status} #{PotionCounter(potion)}";
                await _context.SaveChangesAsync();
                return potion;
            }
            var contextIngredients = _context.Ingredients.ToList();
            var potionIngredients = potion.Ingredients.ToList();
            var backupIngredients = new HashSet<Ingredient>();
            foreach (var ingredient in contextIngredients)
            {
                foreach (var potionIngredient in potionIngredients)
                {
                    if (ingredient.Id == potionIngredient.Id)
                    {
                        backupIngredients.Add(ingredient);
                    }
                }
            }
            potion.Status = BrewingStatus.Discovery;
            potion.Recipe = new Recipe()
            {
                Name = $"{potion.Brewer.Name}'s discovery #{PotionCounter(potion)}",
                Ingredients = backupIngredients,
                Author = potion.Brewer
            };
            potion.Name = potion.Recipe.Name;
            await _context.SaveChangesAsync();
            return potion;
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
    #endregion
}