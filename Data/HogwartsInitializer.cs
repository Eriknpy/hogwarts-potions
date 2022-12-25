using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using HogwartsPotions.Data.Enums;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.Emit;
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

                // Rooms
                Room room101 = new Room() { Capacity = 4 };
                Room room102 = new Room() { Capacity = 4 };
                Room room103 = new Room() { Capacity = 4 };
                Room room104 = new Room(){ Capacity = 4};

                // Students
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

                // Ingredients
                Ingredient ingredientMandrake = new Ingredient() { Name = "Mandrake" };
                Ingredient ingredientUnicordnBlood = new Ingredient() { Name = "UnicordnBlood" };
                Ingredient ingredientUnicordnHair = new Ingredient() { Name = "UnicordnHair" };
                Ingredient ingredientBezoar = new Ingredient() { Name = "Bezoar" };
                Ingredient ingredientDittany = new Ingredient() { Name = "Dittany" };

                // Recipes
                Recipe hpRecipe = new Recipe()
                {
                    Name = "HP-Recipe",
                    Author = studentSanyi,
                    Ingredients = new HashSet<Ingredient>()
                    {
                        ingredientDittany,
                        ingredientMandrake,
                        ingredientBezoar,
                        ingredientUnicordnHair,
                        ingredientUnicordnBlood
                    }
                };

                // Potions
                //Potion hpPotion = new Potion()
                //{
                //    Name = "HP-Potion",
                //    Brewer = studentPisti,
                //    Ingredients = new HashSet<Ingredient>()
                //    {
                //        ingredientMandrake,
                //        ingredientUnicordnBlood,
                //        ingredientUnicordnHair,
                //        ingredientBezoar,
                //        ingredientDittany
                //    },
                //    Recipe = hpRecipe,
                //    Status = BrewingStatus.Replica


                //};

                if (!context.Ingredients.Any())
                {
                    context.Ingredients.AddRange(new HashSet<Ingredient>()
                    {
                        ingredientMandrake,
                        ingredientUnicordnBlood,
                        ingredientUnicordnHair,
                        ingredientBezoar,
                        ingredientDittany
                    });
                    context.SaveChanges();
                }

                //if (!context.Potions.Any())
                //{
                //    context.Potions.AddRange(new List<Potion>() { hpPotion });
                //    context.SaveChanges();
                //}

                if (!context.Recipes.Any())
                {
                    context.Recipes.AddRange(new HashSet<Recipe>() { hpRecipe });
                    context.SaveChanges();
                }
            }
        }
    }
}
