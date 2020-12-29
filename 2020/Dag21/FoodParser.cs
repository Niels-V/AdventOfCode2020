using Common;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag21
{
    [TestClass]
    public class FoodParser : LineParser
    {
        static readonly Regex foodRule = new Regex("^((?<ingredient>\\w+)\\s{0,1})+\\(contains ((?<allergen>\\w+)(,\\s){0,1})+\\)$", RegexOptions.Compiled);

        public FoodList ParseFile(string filePath)
        {
            FoodList list = new FoodList();
            foreach (var line in ReadData(filePath))
            {
                var food = new Food();
                list.Foods.Add(food);
                var regexResult = foodRule.Match(line);
                var ingredients = regexResult.Groups.Values.First(g => g.Name == "ingredient").Captures;
                var allergens = regexResult.Groups.Values.First(g => g.Name == "allergen").Captures;

                foreach (Capture ingredient in ingredients)
                {
                    if (!list.Ingredients.ContainsKey(ingredient.Value))
                    {
                        Ingredient ing = new Ingredient { Name = ingredient.Value };
                        list.Ingredients.Add(ing.Name, ing);
                        food.Ingredients.Add(ing);
                    }
                    else
                    {
                        food.Ingredients.Add(list.Ingredients[ingredient.Value]);
                    }
                }
                foreach (Capture allergen in allergens)
                {
                    if(!list.Allergens.ContainsKey(allergen.Value))
                    {
                        Allergen all = new Allergen { Name = allergen.Value };
                        list.Allergens.Add(all.Name, all);
                        food.GivenAllergens.Add(all);
                    }
                    else
                    {
                        food.GivenAllergens.Add(list.Allergens[allergen.Value]);
                    }
                }
            }
            return list;
        }
    
        [TestMethod]
        public void RunTest()
        {
            var list = ParseFile("test.txt");
            Assert.AreEqual(4, list.Foods.Count());
            Assert.AreEqual(3, list.Allergens.Count());
            Assert.AreEqual(7, list.Ingredients.Count());
        }
    }
}
