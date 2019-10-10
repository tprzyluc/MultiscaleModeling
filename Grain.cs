using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal
{
    class Grain
    {
        public int ID;
        public Color color;

        public Grain(int i, int N)
        {
            ID = i;
            int c = 20 + (200 / N) * i;
            
            Random rnd = new Random();
            
            int blue = rnd.Next(25, 200);
            
            int red = 250-c;
            int green = c;
            //int blue = 250;
            color = Color.FromArgb(red, green, blue);
        }
    }
}
