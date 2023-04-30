using Newtonsoft.Json;

namespace XRFAnalyzer.Models
{
    internal class EmissionLine
    {
        [JsonProperty("element")]
        public int Number { get; set; }
        [JsonProperty("line")]
        public string Line { get; set; } = "";
        [JsonProperty("energy")]
        public double Energy { get; set; }
        [JsonProperty("rate")]
        public double TransitionProbability { get; set; }
        public double JumpRatio { get; set; }
        public double Yield { get; set; }

        public override string? ToString()
        {
            return Number + " " + Line;
        }
    }
}
