using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class JumpRatios
    {
        List<Root>? jumpRatios = new List<Root>();
        public class JumpRatio
        {
            public string line { get; set; }
            public double jump_ratio { get; set; }
        }

        public class Root
        {
            public int element { get; set; }
            public List<JumpRatio> jump_ratios { get; set; }
        }

        public void Deserialize(string filepath) 
        {
            string json = File.ReadAllText(filepath);
            jumpRatios = JsonConvert.DeserializeObject<List<Root>>(json);
        }
    }
}
