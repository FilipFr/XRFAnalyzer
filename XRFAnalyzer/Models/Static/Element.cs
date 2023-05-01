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
        public List<double> MassCoefficientEnergies { get; set; } = new();
        public List<double> MassAbsorptionCoefficients { get; set; } = new();
        public List<double> MassAttenuationCoefficients { get; set; } = new();
        

        Element() { }

        public void PopulateEmissionLines(List<EmissionLine>? lines)
        {
            string[] allowedLines = { "KL3", "KM3", "L3M5", "L2M4" };
            if (lines != null)
            {
                EmissionLines = lines.Where(x => x.Number == Number && allowedLines.Contains(x.Line)).ToList();
                foreach (EmissionLine line in EmissionLines) 
                {
                    line.ElementSymbol = Symbol;
                }
            }
        }

        public void SetMassCoefficients(MassCoefficients.Root data) 
        {
            MassCoefficientEnergies = data.energy;
            MassAbsorptionCoefficients = data.mass_absorption_coefficient;
            MassAttenuationCoefficients = data.mass_attenuation_coefficient;
        }

        public override string? ToString()
        {
            return "" + Number + " " + Symbol;
        }
    }
}
