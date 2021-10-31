using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifterMod
{
    public class LiftsObject
    {
        public List<Lift> lifts { get; set; }

        public LiftsObject()
        {
            lifts = new List<Lift>();
        }
    }

}
