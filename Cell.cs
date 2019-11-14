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
        public bool selected;
        public bool boundary;
        public Cell()
        {
            grainID = -1;
            selected = false;
            boundary = false;
        }
    }
}
