using HtmlAgilityPack;
using PantryPlanner.DTOs;
using PantryPlanner.Services;
using PantryPlannerCore.Models;
using System.Globalization;
using System.Xml.Linq;

namespace PantryPlannerCore.Services
{
    public class RecipeScrapeService
    {
        private readonly IngredientService _ingredientService;

        public RecipeScrapeService(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        public RecipeDto ScrapeAllRecipesWebsite(string url)
        {
            var web = new HtmlWeb();

            var document = web.Load(url);

            if (document.DocumentNode.InnerText == "Ebolg - Not Acceptable")
            {
                throw new Exception("allrecipes.com can not be scraped at this time. Please try again later...");
            }

            Recipe recipe = new Recipe();
            recipe.RecipeUrl= url;
            recipe.DateCreated = DateTime.UtcNow;
            recipe.IsPublic = true;

            // name
            var h1 = document.DocumentNode.QuerySelector("h1.article-heading");
            recipe.Name = HtmlEntity.DeEntitize(h1.InnerText);

            // description
            var p = document.DocumentNode.QuerySelector("p.article-subheading");
            recipe.Description = HtmlEntity.DeEntitize(p.InnerText);

            // prep/serving info
            var prepInfos = document.DocumentNode.QuerySelectorAll("div.mntl-recipe-details__item");

            foreach (var item in prepInfos)
            {
                string label = HtmlEntity.DeEntitize(item.QuerySelector("div.mntl-recipe-details__label").InnerText);
                string value = HtmlEntity.DeEntitize(item.QuerySelector("div.mntl-recipe-details__value").InnerText);

                switch(label.ToLower())
                {
                    case "prep time:": recipe.PrepTime = ParseTimeToMinutes(value);
                        break;
                    case "cook time:":
                        recipe.CookTime = ParseTimeToMinutes(value);
                        break;
                    case "servings:":
                        recipe.ServingSize = value;
                        break;
                }
            }

            // directions
            var directionDivs = document.DocumentNode.QuerySelectorAll("li.mntl-sc-block");
            int sortOrder = 1;

            foreach (var dir in directionDivs)
            {
                RecipeStep step = new RecipeStep();
                step.Text = HtmlEntity.DeEntitize(dir.QuerySelector("p").InnerText);
                step.SortOrder = sortOrder++;
                recipe.RecipeSteps.Add(step);
            }

            // ingredients
            var ingrDivs = document.DocumentNode.QuerySelectorAll("li.mntl-structured-ingredients__list-item");
            sortOrder = 1;

            foreach (var item in ingrDivs)
            {
                RecipeIngredient ingredient = new RecipeIngredient();
                ingredient.SortOrder = sortOrder++;

                var spans = item.QuerySelector("p").QuerySelectorAll("span");

                foreach (var span in spans)
                {
                    if (span.GetAttributeValue("data-ingredient-quantity", "false") == "true")
                    {
                        decimal qty = 0;
                        string[]? qtySplit = HtmlEntity.DeEntitize(span.InnerText)?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var q in qtySplit)
                        {
                            qty += CharUnicodeInfo.GetNumericValue(q[0]) == -1 ? 0 : (decimal) CharUnicodeInfo.GetNumericValue(q[0]);
                        }

                        ingredient.Quantity = qty;
                    }
                    else if (span.GetAttributeValue("data-ingredient-unit", "false") == "true")
                    {
                        ingredient.UnitOfMeasure = HtmlEntity.DeEntitize(span.InnerText);
                    }
                    else if (span.GetAttributeValue("data-ingredient-name", "false") == "true")
                    {
                        string ingrName = HtmlEntity.DeEntitize(span.InnerText);
                        int commaIndex = ingrName.IndexOf(',');
                        ingredient.Method = commaIndex >= 0 ? ingrName[(commaIndex+1)..] : "";


                        Ingredient ing = null;
                        var results = _ingredientService.GetIngredientByName(ingrName, "");
                        if (results?.Count > 0)
                        {
                            ing = results[0];
                        }
                        else
                        {
                            ing = new Ingredient();
                            ing.Name = commaIndex >= 0 ? ingrName[..commaIndex] : ingrName;
                        }

                        ingredient.Ingredient = ing;
                        ingredient.IngredientId = ing.IngredientId;
                    }
                }

                recipe.RecipeIngredients.Add(ingredient);
            }

            return new RecipeDto(recipe);
        }

        public int ParseTimeToMinutes(string time)
        {
            // 45 mins
            // 4 hrs
            // 1 hr 25 mins
            int minutes = 0;
            string[] timeSplit = time.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (time.Contains("min") && !time.Contains("hr"))
            {
                // only minutes
                int.TryParse(timeSplit[0], out minutes);
            }
            else if (time.Contains("hr") && !time.Contains("min"))
            {
                // only hours
                int.TryParse(timeSplit[0], out int hours);
                minutes = hours * 60;
            }
            else
            {
                // hours and minutes
                int.TryParse(timeSplit[0], out int hours);
                int.TryParse(timeSplit[2], out minutes);
                minutes += hours * 60;
            }

            return minutes;
        }
    }
}
