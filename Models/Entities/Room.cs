using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HogwartsPotions.Models.Entities
{
    public class Room
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public int Capacity { get; set; }
        public HashSet<Student> Residents { get; set; }
        public Room()
        {
            Residents = new HashSet<Student>();
            SetRoomCapacity();
        }

        public bool IsSpaceLeft()
        {
            int maxRoomCapacity = 4;
            return Capacity < maxRoomCapacity;
        }

        private void SetRoomCapacity()
        {
            Capacity = Residents.Count;
        }

        public void UpdateCapacity(HashSet<Student> students)
        {
            int maxRoomCapacity = 4;
            if (IsSpaceLeft() && (maxRoomCapacity-Capacity >= students.Count))
            {
                Capacity = students.Count;
            }
            throw new Exception("The room is full!");
        }
    }
}
