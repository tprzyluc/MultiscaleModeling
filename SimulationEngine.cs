using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Metal
{
    class SimulationEngine
    {        
        public List<Grain> grainList;
        public List<Grain> selected_grains = new List<Grain>();
        public Cell[,] board;
        public SimulationConfig config;
        public List<int> selected_grain_ids = new List<int>();
        Random rnd = new Random();

        public SimulationEngine(SimulationConfig config, bool is_loaded)
        {
            this.config = config;

            board = new Cell[config.boardSizeX, config.boardSizeY];
            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    board[i, j] = new Cell();
                }
            }

            grainList = new List<Grain>();
            for (int i = 0; i < config.grainsCount; i++)
            {
                grainList.Add(new Grain(i, config.grainsCount));
            }

            if (!is_loaded)
                GenerateRandomSeeds();


        }
        [JsonConstructor]
        public SimulationEngine(SimulationConfig config)
        {
            this.config = config;
           // rnd = new Random();
            
            board = new Cell[config.boardSizeX, config.boardSizeY];
            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    board[i, j] = new Cell();
                }
            }

            grainList = new List<Grain>();
            for (int i = 0; i < config.grainsCount; i++)
            {
                grainList.Add(new Grain(i, config.grainsCount));
            }
           GenerateRandomSeeds();
            

        }

        public void GenerateRandomSeeds()
        {
            Random rnd = new Random();
            foreach (Grain grain in grainList)
            {
                int x = rnd.Next(0, config.boardSizeX);
                int y = rnd.Next(0, config.boardSizeY);
                board[x, y].grainID = grain.ID;
            }
        }
        public void GenerateRandomSeeds_v2()
        {
            Random rnd = new Random();
            foreach (Grain grain in grainList)
            {
                int x = rnd.Next(0, config.boardSizeX);
                int y = rnd.Next(0, config.boardSizeY);
                if (board[x, y].grainID == -1 )
                    board[x, y].grainID = grain.ID;
            }
        }
        public void GenerateRandomState()
        {
            Random rnd = new Random();
            int x = grainList.Count;

            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    //board[i, j] = rnd.Next(1,x);
                }
            }
                
            
        }

        public void GenerateNextStep()
        {
            Cell[,] nextBoard = new Cell[config.boardSizeX, config.boardSizeY];
            for(int i = 0; i < config.boardSizeX; i++)
            {
                for(int j = 0; j < config.boardSizeY; j++)
                {
                    nextBoard[i, j] = new Cell();
                    nextBoard[i, j].grainID = board[i, j].grainID;
                    nextBoard[i, j].selected = board[i, j].selected;
                    nextBoard[i, j].state_changed = board[i, j].state_changed;
                }
            }

            for(int i = 0; i < config.boardSizeX; i++)
            {
                for(int j = 0; j < config.boardSizeY; j++)
                {
                    overtakeCells(nextBoard, i, j);
                }
            }

            board = nextBoard;

        }

        private void overtakeCells(Cell[,] nextBoard, int x, int y)
        {
            List<int> Xs = new List<int>();
            List<int> Ys = new List<int>();
            CalculateNeighorhood(x, y, Xs, Ys);

            List<List<int>> validNeighborhood = new List<List<int>>();
            switch (config.simulationType)
            {
                case 0:
                    //von neumann
                    validNeighborhood = prepareNeumann(Xs, Ys);
                    Xs = validNeighborhood[0];
                    Ys = validNeighborhood[1];

                    overtake_von_neumann(nextBoard, x, y, Xs, Ys);


                    break;
                case 1:
                    //moore
                    validNeighborhood = prepareMoore(Xs, Ys);

                    Xs = validNeighborhood[0];
                    Ys = validNeighborhood[1];

                    overtake_moore(nextBoard, x, y, Xs, Ys);
                    break;
            }

            //Xs = validNeighborhood[0];
            //Ys = validNeighborhood[1];

            //overtake(nextBoard, x, y, Xs, Ys);

        }

        private void CalculateNeighorhood(int x, int y, List<int> Xs, List<int> Ys)
        {
            int[] xParam = { -1, x, -1 };
            int[] yParam = { -1, y, -1 };

          
            calculateCross(xParam, yParam);
            Xs.Add(xParam[0]);
            Ys.Add(yParam[0]);

            Xs.Add(xParam[1]);
            Ys.Add(yParam[0]);

            Xs.Add(xParam[2]);
            Ys.Add(yParam[0]);

            Xs.Add(xParam[0]);
            Ys.Add(yParam[1]);

            Xs.Add(xParam[2]);
            Ys.Add(yParam[1]);

            Xs.Add(xParam[0]);
            Ys.Add(yParam[2]);

            Xs.Add(xParam[1]);
            Ys.Add(yParam[2]);

            Xs.Add(xParam[2]);
            Ys.Add(yParam[2]);
        }

        private void calculateCross(int[] x, int[] y)
        {
          
            if (x[1] - 1 >= 0)
                x[0] = x[1] - 1;
            
            if (x[1] + 1 < config.boardSizeX)
                x[2] = x[1] + 1;

            if (y[1] - 1 >= 0)            
                y[0] = y[1] - 1;
            
            if (y[1] + 1 < config.boardSizeY)
                y[2] = y[1] + 1;
            
        }

     

        private void overtake_von_neumann(Cell[,] nextBoard, int x, int y, List<int> Xs, List<int> Ys)
        {
            if (!board[x, y].selected)
            {
                if (board[x, y].grainID >= 0)
                {
                    for (int i = 0; i < Xs.Count; i++)
                    {
                        if (Xs[i] >= 0 && Ys[i] >= 0)
                        {
                            if (board[Xs[i], Ys[i]].grainID == -1)
                            {
                                if (nextBoard[Xs[i], Ys[i]].grainID != -1)
                                {
                                    if (nextBoard[Xs[i], Ys[i]].grainID == -2)
                                        continue;
                                    if (board[Xs[i], Ys[i]].selected)
                                        continue;
                                    if (nextBoard[Xs[i], Ys[i]].selected)
                                        continue;

                                    int[] color_lead = new int[Xs.Count];



                                    if (Xs[0] >= 0 && Ys[0] >= 0)
                                    {
                                        color_lead[0] = board[Xs[0], Ys[0]].grainID;
                                    }
                                    if (Xs[1] >= 0 && Ys[1] >= 0)
                                    {
                                        color_lead[1] = board[Xs[1], Ys[1]].grainID;
                                    }

                                    if (Xs[2] >= 0 && Ys[2] >= 0)
                                    {
                                        color_lead[2] = board[Xs[2], Ys[2]].grainID;
                                    }

                                    if (Xs[3] >= 0 && Ys[3] >= 0)
                                    {
                                        color_lead[3] = board[Xs[3], Ys[3]].grainID;
                                    }


                                    var k = (from numbers in color_lead
                                             group numbers by numbers into grouped
                                             select new { Number = grouped.Key, Freq = grouped.Count() }).FirstOrDefault();

                                    if (k.Number == -1)
                                        continue;
                                    if (k.Number == -2)
                                        continue;



                                    nextBoard[Xs[i], Ys[i]].grainID = k.Number;
                                }
                                else
                                {
                                    if (nextBoard[Xs[i], Ys[i]].grainID == -2)
                                        continue;
                                    if (board[Xs[i], Ys[i]].selected)
                                        continue;
                                    if (nextBoard[Xs[i], Ys[i]].selected)
                                        continue;
                                    nextBoard[Xs[i], Ys[i]].grainID = board[x, y].grainID;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void overtake_moore(Cell[,] nextBoard, int x, int y, List<int> Xs, List<int> Ys)
        {
          
            if (board[x, y].grainID >= 0)
            {
                for (int i = 0; i < Xs.Count; i++)
                {
                    if (Xs[i] >= 0 && Ys[i] >= 0)
                    {
                        if (board[Xs[i], Ys[i]].grainID == -1)
                        {
                            if (nextBoard[Xs[i], Ys[i]].grainID != -1)
                            {
                                if (nextBoard[Xs[i], Ys[i]].grainID == -2)
                                    continue;

                                int[] color_lead = { -1, -1, -1, -1, -1, -1, -1, -1 };
                                int[] color_lead_2 = { -1, -1, -1, -1};
                                int[] color_lead_3 = { -1, -1, -1, -1};


                            

                                object balanceLock = new object();
                                for (int j = 0; j < Xs.Count; j++)
                                    if (Xs[j] >= 0 && Ys[j] >= 0)
                                        color_lead[j] = board[Xs[j], Ys[j]].grainID;



                                
                                
                                    var k = (from numbers in color_lead
                                             group numbers by numbers into grouped
                                             select new { Number = grouped.Key, Freq = grouped.Count() }).FirstOrDefault();
                                


                                if (k.Freq < 5)
                                {
                                    int a = 0;
                                    //for (int j = 1; j < Xs.Count; j += 2)
                                    //    if (Xs[j] >= 0 && Ys[j] >= 0)
                                    //    {
                                    //        color_lead_2[a] = board[Xs[j], Ys[j]].grainID;
                                    //        a++;
                                    //    } 

                                    if (Xs[1] >= 0 && Ys[1] >= 0)
                                    {
                                        color_lead_2[a] = board[Xs[1], Ys[1]].grainID;

                                        a++;
                                    }
                                    if (Xs[3] >= 0 && Ys[3] >= 0)
                                    {
                                        color_lead_2[a] = board[Xs[3], Ys[3]].grainID;

                                        a++;
                                    }
                                    if (Xs[4] >= 0 && Ys[4] >= 0)
                                    {
                                        color_lead_2[a] = board[Xs[4], Ys[4]].grainID;

                                        a++;
                                    }
                                    if (Xs[6] >= 0 && Ys[6] >= 0)
                                    {
                                        color_lead_2[a] = board[Xs[6], Ys[6]].grainID;

                                        a++;
                                    }

                                    var m = (from numbers in color_lead_2
                                             group numbers by numbers into grouped
                                             select new { Number = grouped.Key, Freq = grouped.Count() }).FirstOrDefault();
                                    if (m.Freq < 3)
                                    {
                                        a = 0;
                                        //for (int j = 0; j < Xs.Count; j += 2)
                                            if (Xs[0] >= 0 && Ys[0] >= 0)
                                            {
                                                color_lead_3[a] = board[Xs[0], Ys[0]].grainID;

                                                a++;
                                            }
                                            if (Xs[2] >= 0 && Ys[2] >= 0)
                                            {
                                                color_lead_3[a] = board[Xs[2], Ys[2]].grainID;

                                                a++;
                                            }
                                            if (Xs[5] >= 0 && Ys[5] >= 0)
                                            {
                                                color_lead_3[a] = board[Xs[5], Ys[5]].grainID;

                                                a++;
                                            }
                                            if (Xs[7] >= 0 && Ys[7] >= 0)
                                            {
                                                color_lead_3[a] = board[Xs[7], Ys[7]].grainID;

                                                a++;
                                            }



                                        var n = (from numbers in color_lead_3
                                                 group numbers by numbers into grouped
                                                 select new { Number = grouped.Key, Freq = grouped.Count() }).FirstOrDefault();


                                        if (n.Freq < 3)
                                        {
                                            //Thread.Sleep(10);
                                            int chance;                                           
                                            
                                            chance =  rnd.Next(1, 100);
                                            if (chance >= config.probability)
                                            {
                                                for (int j = 0; j < Xs.Count; j++)
                                                    if (Xs[j] >= 0 && Ys[j] >= 0)
                                                        color_lead[j] = board[Xs[j], Ys[j]].grainID;

                                                k = (from numbers in color_lead
                                                     group numbers by numbers into grouped
                                                     select new { Number = grouped.Key, Freq = grouped.Count() }).FirstOrDefault();

                                                nextBoard[Xs[i], Ys[i]].grainID = k.Number;
                                            }
                                            else
                                            {
                                                
                                            }
                                                


                                        }
                                        else
                                        {
                                            nextBoard[Xs[i], Ys[i]].grainID = n.Number;
                                        }
                                    }
                                    else
                                    {
                                        //if (m.Number == -1)
                                        //    continue;
                                        //if (m.Number == -2)
                                        //    continue;



                                        nextBoard[Xs[i], Ys[i]].grainID = m.Number;
                                    }
                                }
                                else
                                {
                                    //if (k.Number == -1)
                                    //    continue;
                                    //if (k.Number == -2)
                                    //    continue;



                                    nextBoard[Xs[i], Ys[i]].grainID = k.Number;
                                }

                              
                            }
                            else
                            {
                                //if (nextBoard[Xs[i], Ys[i]].grainID == -2)
                                //    continue;
                                nextBoard[Xs[i], Ys[i]].grainID = board[x, y].grainID;
                            }
                        }
                    }
                }
            }
        }



        private List<List<int>> prepareNeumann(List<int> X, List<int> Y)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();

            x.Add(X[1]);
            y.Add(Y[1]);

            x.Add(X[3]);
            y.Add(Y[3]);

            x.Add(X[4]);
            y.Add(Y[4]);

            x.Add(X[6]);
            y.Add(Y[6]);

            List<List<int>> tmp = new List<List<int>>();
            tmp.Add(x);
            tmp.Add(y);
            return tmp;
        }
        private List<List<int>> prepareMoore(List<int> X, List<int> Y)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();

            for(int i = 0; i < X.Count; i++)
            {
                x.Add(X[i]);
                y.Add(Y[i]);
            }
            
            List<List<int>> tmp = new List<List<int>>();
            tmp.Add(x);
            tmp.Add(y);
            return tmp;
        }


        public int CountEmptyCells()
        {
            int counter = 0;
            for(int i = 0; i < config.boardSizeX; i++)
            {
                for(int j = 0; j < config.boardSizeY; j++)
                {
                    if (board[i, j].grainID == -1)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

       

        //public void AddSeed(int x, int y)
        //{
        //    config.grainsCount++;
        //    grainList.Add(new Grain(config.grainsCount - 1, config.grainsCount));
        //    int X = x / config.grainXsize;
        //    int Y = y / config.grainYsize;
        //    board[X, Y].grainID = config.grainsCount - 1;
        //}

        public void CA_CA(int x,int y)
        {
            config.selected_grains_counter++;
            selected_grain_ids.Add(board[x, y].grainID);
            selected_grains.Add(new Grain(board[x,y].grainID,Color.Pink,x,y));
          
        }

    }
}
