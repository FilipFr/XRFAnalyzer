using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class QualitativeAnalysis
    {

        public List<EmissionLine> Results { get; set; } = new();


        public void ConfirmResults(List<Element> confirmedLines) 
        {

        }
    }
}
