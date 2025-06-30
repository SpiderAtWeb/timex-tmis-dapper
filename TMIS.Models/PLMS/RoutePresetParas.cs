using System.Text.Json.Serialization;

namespace TMIS.Models.PLMS
{
    public class RoutePresetParas
    {
        public int PresetId { get; set; }
        public string SelectedDate { get; set; } = string.Empty;
    }
}
