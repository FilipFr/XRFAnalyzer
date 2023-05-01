using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

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
        public string? ElementSymbol { get; set; }

        public override string? ToString()
        {
            if (ElementSymbol != null)
            { 
                return ElementSymbol + " " + Line;
            } 
            return Number + " " + Line;
        }

        public double GetTransitionProbability(List<EmissionLine> lines, double detectorResolution) 
        {
            if (lines != null && lines.Count != 0) 
            {
                double probability = 0;
                List<EmissionLine> emissionLines = lines.Where(x =>x.Number == this.Number).ToList();
                foreach(EmissionLine emissionLine in emissionLines) 
                {
                    if(this.Energy - detectorResolution < emissionLine.Energy && emissionLine.Energy < this.Energy + detectorResolution)
                    {
                        probability += emissionLine.Energy;
                    }
                }
                if (probability > 0) 
                {
                    return probability;
                }
            }
            return -1;
        }
    }
}
