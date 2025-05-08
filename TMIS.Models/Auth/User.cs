namespace TMIS.Models.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string NameWi { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public int[]? AccessPlants { get; set; }

    }
}
