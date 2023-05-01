using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class Yields
    {
        public List<Root>? Data { get; set; } = new List<Root>();
        
        public class Probability
        {
            public string line { get; set; } = "";
            public double probability { get; set; }
        }

        public class Root
        {
            public int element { get; set; }
            public List<Probability> probabilities { get; set; } = new();
        }
        public void Deserialize(string filepath) 
        {
            string json = File.ReadAllText(filepath);
            Data = JsonConvert.DeserializeObject<List<Root>>(json);
        }

        public double GetValueForLine(EmissionLine line)
        {
            if (line == null)
            {
                return -1;
            }
            Yields.Root? yieldRoot = Data?.FirstOrDefault(x => x.element == line.Number);
            if (yieldRoot != null && yieldRoot.probabilities != null)
            {
                Yields.Probability? probability = yieldRoot.probabilities.FirstOrDefault(x => line.Line.StartsWith(x.line));
                if (probability != null)
                {
                    return probability.probability;
                }
            }
            return -1;
        }
    }
}
