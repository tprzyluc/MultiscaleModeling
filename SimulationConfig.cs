using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal
{
    class SimulationConfig
    {
        public bool pbc;
        public enum neighborhood { von_neumann, moore, hex_left, hex_right, hex_rand, pent_rand};
        public int simulationType;
        public int boardSizeX;
        public int boardSizeY;
        public int grainXsize;
        public int grainYsize;
        public int grainsCount;
        public int minimalGrainsDistance;
        public int seedPlacement;

        public SimulationConfig(int X, int Y, int count, neighborhood type, int distance)
        {
            boardSizeX = X;
            boardSizeY = Y;
            grainsCount = count;
            simulationType = (int)type;
            minimalGrainsDistance = distance;
        }

        public SimulationConfig()
        {

        }



    }
}
