using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.CodeDom;

namespace XRFAnalyzer.Models
{
    internal class Detector
    {
        [JsonProperty("detector")]
        public string Name { get; set; } = "";
        [JsonProperty("window")]
        public string Window { get; set; } = "";
        [JsonProperty("energy")]
        private List<double> Energies { get; set; } = new();
        [JsonProperty("total_attenuation")]
        private List<double> TotalAttenuations { get; set; } = new();

        public static List<Detector> LoadData(string filepath) 
        {
            var detectors = JsonConvert.DeserializeObject<List<Detector>>(File.ReadAllText(filepath));
            if (detectors == null || detectors.Count == 0)
            { 
                return new(); 
            }
            return detectors ;
        }

        public override string ToString()
        {
            return Name + ", " + Window + " window";
        }


    }
}
