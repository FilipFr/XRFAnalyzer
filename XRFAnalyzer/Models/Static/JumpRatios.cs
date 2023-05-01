using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static XRFAnalyzer.Models.JumpRatios;

namespace XRFAnalyzer.Models
{
    internal class JumpRatios
    {
        public List<Root>? Data { get; set; } = new List<Root>();
        public class JumpRatio
        {
            public string line { get; set; } = "";
            public double jump_ratio { get; set; }
        }

        public class Root
        {
            public int element { get; set; }
            public List<JumpRatio> jump_ratios { get; set; } = new();
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
            JumpRatios.Root? jumpRatioRoot = Data?.FirstOrDefault(x => x.element == line.Number);
            if (jumpRatioRoot != null && jumpRatioRoot.jump_ratios != null) 
            {
                JumpRatios.JumpRatio? jump_ratio = jumpRatioRoot.jump_ratios.FirstOrDefault(x => line.Line.StartsWith(x.line));
                if (jump_ratio != null) 
                {
                    return jump_ratio.jump_ratio;
                }
                
            }
            return -1;
        }
    }
}
