using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static XRFAnalyzer.Models.JumpRatios;

namespace XRFAnalyzer.Models
{
    internal class Parameters
    {
        public Yields Yields { get; set; }
        public JumpRatios JumpRatios { get; set; }
        public MassCoefficients MassCoefficients { get; set; }

        public Parameters() 
        {
            Yields = new Yields();
            JumpRatios = new JumpRatios();
            MassCoefficients = new MassCoefficients();
            Yields.DeserializeYields("Resources\\Data\\fluorescent_yield.json");
            JumpRatios.Deserialize("Resources\\Data\\jump_ratio.json");
            MassCoefficients.Deserialize("Resources\\Data\\xray_mass_ceoficient.json");
        }
    }
}
