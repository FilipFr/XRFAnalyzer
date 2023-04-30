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
        List<Root>? yields = new List<Root>();
        
        public class Probability
        {
            public string line { get; set; }
            public double probability { get; set; }
        }

        public class Root
        {
            public int element { get; set; }
            public List<Probability> probabilities { get; set; }
        }
        public void DeserializeYields(string filepath) 
        {
            string json = File.ReadAllText(filepath);
            yields = JsonConvert.DeserializeObject<List<Root>>(json);
        }
    }
}
