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
        }
    }
}