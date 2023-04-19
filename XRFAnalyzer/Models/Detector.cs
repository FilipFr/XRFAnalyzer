using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        private List<double> TotalAttenuation { get; set; } = new();

        public List<Detector> LoadData(string filepath) 
        {
            List<Detector>? detectors = JsonConvert.DeserializeObject<List<Detector>>(File.ReadAllText("Resources\\Data\\elements_lines.json"));
            if (detectors == null || detectors.Count == 0)
            { 
                return new(); 
            }
            
            return detectors ;
        }

    }
}
