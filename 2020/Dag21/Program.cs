using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag21
{
    [TestCategory("2020")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static int First(string inputFile)
        {
            var parser = new FoodParser();
            var list = parser.ParseFile(inputFile);

            while (list.Allergens.Count(all => all.Value.Ingredient == null) > 0)
            {
                foreach (var allergen in list.Allergens.Where(all => all.Value.Ingredient == null))
                {
                    var foodWithAllergen = list.Foods.Where(f => f.GivenAllergens.Contains(allergen.Value));
                    var ingredientsWithPossibleAllergen = foodWithAllergen.SelectMany(f => f.Ingredients).Distinct();

                    var ingredientsInAllFoodsWithAllergen = ingredientsWithPossibleAllergen.Where(i => foodWithAllergen.All(f => f.Ingredients.Contains(i)));

                    if (ingredientsInAllFoodsWithAllergen.Count() == 1)
                    {
                        ingredientsInAllFoodsWithAllergen.Single().Allergen = allergen.Value;
                        allergen.Value.Ingredient = ingredientsInAllFoodsWithAllergen.Single();
                    }
                }
                //Calculate new food list without known ingredients and allergens
                list.CleanFoodList();

                //do allergen match again
            }
            var foodIngredientsWithoutAllergen = list.Foods.Sum(f => f.Ingredients.Count());
            return foodIngredientsWithoutAllergen;
        }

        static string Second(string inputFile)
        {
            var parser = new FoodParser();
            var list = parser.ParseFile(inputFile);

            while (list.Allergens.Count(all => all.Value.Ingredient == null) > 0)
            {
                foreach (var allergen in list.Allergens.Where(all => all.Value.Ingredient == null))
                {
                    var foodWithAllergen = list.Foods.Where(f => f.GivenAllergens.Contains(allergen.Value));
                    var ingredientsWithPossibleAllergen = foodWithAllergen.SelectMany(f => f.Ingredients).Distinct();

                    var ingredientsInAllFoodsWithAllergen = ingredientsWithPossibleAllergen.Where(i => foodWithAllergen.All(f => f.Ingredients.Contains(i)));

                    if (ingredientsInAllFoodsWithAllergen.Count() == 1)
                    {
                        ingredientsInAllFoodsWithAllergen.Single().Allergen = allergen.Value;
                        allergen.Value.Ingredient = ingredientsInAllFoodsWithAllergen.Single();
                    }
                }
                //Calculate new food list without known ingredients and allergens
                list.CleanFoodList();

                //do allergen match again
            }
            
            var ingredientList = string.Join(",",list.Ingredients.Where(ing => ing.Value.Allergen != null).OrderBy(ing => ing.Value.Allergen.Name).Select(ing => ing.Key));
            return ingredientList;
        }


        [DataTestMethod]
        [DataRow("test.txt", 5)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", "mxmxvkd,sqjhc,fvjkl")]
        public void TestPart2(string inputFile, string expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
