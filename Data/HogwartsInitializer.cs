using System.Collections.Generic;
using System.Linq;
using HogwartsPotions.Data.Enums;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HogwartsPotions.Data
{
    public class HogwartsInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateAsyncScope())
            {
                var context = serviceScope.ServiceProvider.GetService<HogwartsContext>();
                context.Database.EnsureCreated();

                #region Rooms
                var room101 = new Room() { Capacity = 4 };
                var room102 = new Room() { Capacity = 4 };
                var room103 = new Room() { Capacity = 4 };
                var room104 = new Room() { Capacity = 4 };
                #endregion

                #region Students
                Student studentPisti = new Student()
                {
                    Name = "Pisti",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.Cat,
                    Room = room101
                };
                Student studentSanyi = new Student()
                {
                    Name = "Sanyi",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.None,
                    Room = room101
                };
                Student studentJózsi = new Student()
                {
                    Name = "Józsi",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.Owl,
                    Room = room102
                };
                Student studentMarci = new Student()
                {
                    Name = "Marci",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.Rat,
                    Room = room103
                };
                #endregion

                #region Ingredients
                var Mandrake = new Ingredient() { Name = "Mandrake" };
                var UnicornBlood = new Ingredient() { Name = "UnicornBlood" };
                var UnicornHair = new Ingredient() { Name = "UnicornHair" };
                var Bezoar = new Ingredient() { Name = "Bezoar" };
                var Dittany = new Ingredient() { Name = "Dittany" };
                #endregion

                #region Recipes
                Recipe studentSanyiDiscovery1 = new Recipe()
                {
                    Name = $"{studentSanyi.Name}'s discovery #1",
                    Author = studentSanyi,
                    Ingredients = new HashSet<Ingredient>()
                    {
                        Dittany,
                        Mandrake,
                        Bezoar,
                        UnicornHair,
                        UnicornBlood
                    }
                };
                #endregion

                #region Context
                if (!context.Rooms.Any())
                {
                    context.Rooms.AddRange(new HashSet<Room>()
                    {
                        room101,
                        room102,
                        room103,
                        room104
                    });
                    context.SaveChanges();
                }

                if (!context.Students.Any())
                {
                    context.Students.AddRange(new HashSet<Student>()
                    {
                        studentPisti,
                        studentSanyi,
                        studentJózsi,
                        studentMarci
                    });
                    context.SaveChanges();
                }

                if (!context.Ingredients.Any())
                {
                    context.Ingredients.AddRange(new HashSet<Ingredient>()
                    {
                        Mandrake,
                        UnicornHair,
                        UnicornBlood,
                        Bezoar,
                        Dittany
                    });
                    context.SaveChanges();
                }

                if (!context.Recipes.Any())
                {
                    context.Recipes.AddRange(new HashSet<Recipe>() { studentSanyiDiscovery1 });
                    context.SaveChanges();
                }
                #endregion
            }
        }
    }
}