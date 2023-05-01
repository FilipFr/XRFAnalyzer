using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XRFAnalyzer.Models.Static
{
    internal partial class ElementData : ObservableObject
    {
        [ObservableProperty]
        private List<Element> _data = new();

        public ElementData()
        {
            try
            {
                List<EmissionLine>? lines = JsonConvert.DeserializeObject<List<EmissionLine>>(File.ReadAllText("Resources\\Data\\elements_lines.json"));
                List<Element>? elements = JsonConvert.DeserializeObject<List<Element>>(File.ReadAllText("Resources\\Data\\elements_info.json"));
                Yields yields = new Yields();
                JumpRatios jumpRatios = new JumpRatios();
                MassCoefficients massCoefficients = new MassCoefficients();
                yields.Deserialize("Resources\\Data\\fluorescent_yield.json");
                jumpRatios.Deserialize("Resources\\Data\\jump_ratio.json");
                massCoefficients.Deserialize("Resources\\Data\\xray_mass_ceoficient.json");

                if (elements == null || lines == null || yields.Data == null || jumpRatios.Data == null || massCoefficients.Data == null)
                {
                    throw new Exception("Element data could not be loaded.");
                }

                foreach (Element element in elements)
                {
                    element.PopulateEmissionLines(lines);
                    if (element.EmissionLines.Count > 0)
                    {
                        foreach(EmissionLine line in element.EmissionLines) 
                        {
                            line.JumpRatio = jumpRatios.GetValueForLine(line);
                            line.Yield = yields.GetValueForLine(line);
                            line.TransitionProbability = line.GetTransitionProbability(element.EmissionLines, 0.15);
                        }
                        this.Data.Add(element);
                    }
                    MassCoefficients.Root? root = massCoefficients.Data.FirstOrDefault(x => x.element == element.Number);
                    if (root != null) 
                    {
                        element.SetMassCoefficients(root);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nCheck if resource file isn't missing from Resources\\Data folder." );
            }
        }
    }
}
