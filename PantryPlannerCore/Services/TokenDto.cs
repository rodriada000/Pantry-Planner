namespace PantryPlanner.Services
{
    public class TokenDto
    {
        public TokenDto()
        {
        }

        public string? Token { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}