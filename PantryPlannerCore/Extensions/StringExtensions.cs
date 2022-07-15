namespace PantryPlannerCore.Extensions
{
    public static class StringExtensions
    {
        public static string ToProperCase(this string text)
        {
            System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }
    }
}
