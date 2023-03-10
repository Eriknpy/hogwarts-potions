using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HogwartsPotions.Models.Entities
{
    public class Recipe
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public Student Author { get; set; }
        public HashSet<Ingredient> Ingredients { get; set; }
        public Recipe()
        {
            Ingredients = new HashSet<Ingredient>();
        }
    }
}