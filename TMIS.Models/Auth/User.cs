using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string? ShortName { get; set; } = string.Empty;
        public string[] UserRolesList { get; set; } = [];
        public int[] UserLocationList { get; set; } = [];

    }
}
