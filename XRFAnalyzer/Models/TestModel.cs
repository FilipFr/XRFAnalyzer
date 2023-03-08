using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class TestModel
    {
        public int Id { get; set; }

        public void ChangeId(int id)
        {
            Id = id;
        }
    }
}
