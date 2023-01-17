using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HogwartsPotions.Models.Entities
{
    public class Room
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int Capacity { get; set; }
        public HashSet<Student> Residents { get; set; }
        public Room()
        {
            Residents = new HashSet<Student>();
        }
    }
}