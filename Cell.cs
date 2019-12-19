using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal
{
    class Cell
    {
        public int grainID;
        public double energy;
        public bool selected;
        public bool boundary;
        public bool state_changed;
        public Cell()
        {
            grainID = -1;
            selected = false;
            boundary = false;
            state_changed = false;
            energy = -1;

        }
    }
}
