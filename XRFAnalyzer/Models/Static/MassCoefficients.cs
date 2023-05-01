using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class MassCoefficients
    {
        public List<Root>? Data = new List<Root>();
        public class Root
        {
            public int element { get; set; }
            public string energy_unit { get; set; } = "";
            public List<double> energy { get; set; } = new();
            public string mass_attenuation_coefficient_unit { get; set; } = "";
            public List<double> mass_attenuation_coefficient { get; set; } = new();
            public string mass_absorption_coefficient_unit { get; set; } = "";
            public List<double> mass_absorption_coefficient { get; set; } = new();
        }

        public void Deserialize(string filepath) 
        {
            string json = File.ReadAllText(filepath);
            Data = JsonConvert.DeserializeObject<List<Root>>(json);
        }
        
    }
}
