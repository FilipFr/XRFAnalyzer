using Newtonsoft.Json;

namespace XRFAnalyzer.Models
{
    internal class EmissionLine
    {
        [JsonProperty("element")]
        public int AtomicNumber { get; set; }
        [JsonProperty("line")]
        public string Line { get; set; } = "";
        [JsonProperty("energy")]
        public double Energy { get; set; }

        public override string? ToString()
        {
            return Line;
        }
    }
}
