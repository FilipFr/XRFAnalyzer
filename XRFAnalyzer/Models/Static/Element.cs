using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XRFAnalyzer.Models
{
    internal class Element
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";
        [JsonProperty("number")]
        public int Number { get; set; }
        public List<EmissionLine> EmissionLines { get; set; } = new List<EmissionLine>();

        Element() { }

        public void PopulateEmissionLines(List<EmissionLine>? lines)
        {
            if (lines != null)
            {
                EmissionLines = lines.Where(x => x.Number == Number).ToList();
            }
        }

        public override string? ToString()
        {
            return "" + Number + " " + Symbol;
        }
    }
}
