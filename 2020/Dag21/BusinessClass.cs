using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dag21
{
    public class Food
    {
        public List<Ingredient> Ingredients { get; }
        public List<Allergen> GivenAllergens { get; }
        public Food()
        {
            Ingredients = new List<Ingredient>();
            GivenAllergens = new List<Allergen>();
        }
    }

    public class FoodList
    {
        public Dictionary<string, Ingredient> Ingredients { get; }
        public Dictionary<string, Allergen> Allergens { get; }
        public List<Food> Foods { get; }

        public FoodList()
        {
            Ingredients = new Dictionary<string, Ingredient>();
            Allergens = new Dictionary<string, Allergen>();
            Foods = new List<Food>();
        }

        public void CleanFoodList()
        {
            foreach (var food in Foods)
            {
                var foundIngredientsWithAllergens = food.Ingredients.Where(ingr => ingr.Allergen != null);
                var foundAllergens = foundIngredientsWithAllergens.Select(i => i.Allergen).ToList();
                food.Ingredients.RemoveAll(ingr => ingr.Allergen != null);
                food.GivenAllergens.RemoveAll(all => foundAllergens.Contains(all));
            }
        }
    }

    public class Allergen
    {
        public string Name { get; set; }
        public Ingredient Ingredient { get; set; }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public Allergen Allergen { get; set; }
    }
}
