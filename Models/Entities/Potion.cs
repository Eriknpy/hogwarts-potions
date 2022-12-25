using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using HogwartsPotions.Data.Enums;

namespace HogwartsPotions.Models.Entities
{
    public class Potion
    {
        public long Id { get; set; }
        public Student Brewer { get; set; }
        public string Name { get; set; }
        public HashSet<Ingredient> Ingredients { get; set; }
        public Recipe Recipe { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BrewingStatus Status { get; set; }

        public Potion()
        {
            Ingredients = new HashSet<Ingredient>();
            Status = BrewingStatus.Brew;
            //Name = $"{Brewer.Name}'s Potions";
            //SetBrewingStatus();
        }

        //public void SetBrewingStatus()
        //{
        //    if (Ingredients.Count < 5)
        //    {
        //        Status = BrewingStatus.Brew;
        //    }

        //    if (CompareIngredientLists())
        //    {
        //        Status = BrewingStatus.Replica;
        //    }
        //    Status = BrewingStatus.Discovery;
        //}

        //public bool CompareIngredientLists()
        //{
        //    List<string> PotionIngredients = new List<string>();

        //    foreach (var ingredient in Ingredients)
        //    {
        //        PotionIngredients.Add(ingredient.Name);
        //    }

        //    List<string> RecipeIngredients = new List<string>();

        //    foreach (var ingredient in Recipe.Ingredients)
        //    {
        //        RecipeIngredients.Add(ingredient.Name);
        //    }

        //    bool isEqual = !RecipeIngredients.Except(PotionIngredients).Any();

        //    if (PotionIngredients.Count != RecipeIngredients.Count)
        //    {
        //        return false;
        //    }
        //    return isEqual;
        //}
    }
}
