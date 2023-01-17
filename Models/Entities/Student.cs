using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HogwartsPotions.Data.Enums;

namespace HogwartsPotions.Models.Entities
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HouseType HouseType { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetType PetType { get; set; }
        public Room Room { get; set; }
    }
}