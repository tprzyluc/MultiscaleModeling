using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal
{
    class SimulationEngine
    {        
        public List<Grain> grainList;
        public Cell[,] board;
        public SimulationConfig config;
        Random rnd;

        

        public SimulationEngine(SimulationConfig config)
        {
            this.config = config;
            rnd = new Random();

            //stwórz przestrzen automatu
            board = new Cell[config.boardSizeX, config.boardSizeY];
            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    board[i, j] = new Cell();
                }
            }

            //stwórz listę ziaren
            grainList = new List<Grain>();
            for (int i = 0; i < config.grainsCount; i++)
            {
                grainList.Add(new Grain(i, config.grainsCount));
            }

            //rozmieść zarodki
            switch(config.seedPlacement)
            {
                case 0:
                    //losowe
                    GenerateRandomSeeds();
                    break;
                case 1:
                    //równomierne
                    GenerateEqualSeeds();
                    break;
                case 2:
                    //losowe z promieniem
                    GenerateRandomSeedsWithDistance();
                    break;
            }


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

        public void GenerateEqualSeeds()
        {
            double n = Math.Sqrt(config.grainsCount);
            if (Math.Floor(n) != n)
            {
                //musze znaleźć nabliższą libcze posiadającą pierwiastek
                double c1 = config.grainsCount;
                double c2 = config.grainsCount;
                do
                {
                    c1++;
                    c2--;
                    if (Math.Floor(Math.Sqrt(c1)) == Math.Sqrt(c1))
                    {
                        n = Math.Sqrt(c1);
                        break;
                    }
                    if (Math.Floor(Math.Sqrt(c2)) == Math.Sqrt(c2))
                    {
                        n = Math.Sqrt(c2);
                        break;
                    }
                } while (true);
            }
            //mogę podzielić zarodki na planszę nxn
            config.grainsCount = (int)(n * n);
            grainList = new List<Grain>();
            for (int i = 0; i < config.grainsCount; i++)
            {
                grainList.Add(new Grain(i, config.grainsCount));
            }

            int xSpan = (int)Math.Floor(config.boardSizeX / (n + 1));
            int ySpan = (int)Math.Floor(config.boardSizeY / (n + 1));

            int counter = 0;

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    board[(i + 1) * xSpan, (j + 1) * ySpan].grainID = grainList[counter].ID;
                    counter++;
                }
            }

        }

        public void GenerateRandomSeedsWithDistance()
        {
            List<int> Xs = new List<int>();
            List<int> Ys = new List<int>();
            int x;
            int y;
            int counter = 0;
            double distance= 0;
            Random rnd = new Random();
            for (int i=0 ; i < grainList.Count;i++)
            {
                do {
                    distance = config.minimalGrainsDistance;
                    if (counter == 100000)
                    {
                        //prawdopodobnie nie uda się umieścić tego zarodka
                        System.Windows.Forms.MessageBox.Show("Po 1000 prób nie udało się umieści zarodka nr "+i,"Błąd");
                    }
                    x = rnd.Next(0, config.boardSizeX);
                    y = rnd.Next(0, config.boardSizeY);
                    for(int j = 0; j < i; j++)
                    {
                        double dst = Math.Sqrt(Math.Pow(x - Xs[j], 2) + Math.Pow(y - Ys[j], 2));
                        if (dst < distance)
                            distance = dst;
                    }
                    counter++;
                    if (i == 0)
                        distance = config.minimalGrainsDistance;
                } while (distance<config.minimalGrainsDistance);
                Xs.Add(x);
                Ys.Add(y);
                board[x, y].grainID = grainList.ElementAt(i).ID;
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
            //znajdź komplet 8 sąiadów
            List<int> Xs = new List<int>();
            List<int> Ys = new List<int>();
            CalculateNeighorhood(x, y, Xs, Ys);

            //w lazeżności od wybranego sąsiedztwa zajmij komórki
            List<List<int>> validNeighborhood = new List<List<int>>();
            switch (config.simulationType)
            {
                case 0:
                    //von neumann
                    validNeighborhood = prepareNeumann(Xs, Ys);
                    break;
                case 1:
                    //moore
                    validNeighborhood = prepareMoore(Xs, Ys);
                    break;
                case 2:
                    //hex left
                    validNeighborhood = prepareHexLeft(Xs, Ys);
                    break;
                case 3:
                    //hex right
                    validNeighborhood = prepareHexRight(Xs, Ys);
                    break;
                case 4:
                    //hex random
                    validNeighborhood = prepareHexRand(Xs, Ys);
                    break;
                case 5:
                    //pent random
                    validNeighborhood = preparePentRandom(Xs, Ys);
                    break;

            }

            Xs = validNeighborhood[0];
            Ys = validNeighborhood[1];

            overtake(nextBoard, x, y, Xs, Ys);

        }

        private void CalculateNeighorhood(int x, int y, List<int> Xs, List<int> Ys)
        {
            int[] xParam = { -1, x, -1 };
            int[] yParam = { -1, y, -1 };

            //policz współrzędne punktów tworzących krzyż
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
            if (config.pbc)
            {
                if (x[1] - 1 < 0)
                {
                    x[0] = config.boardSizeX - 1;
                }
                else
                {
                    x[0] = x[1] - 1;
                }

                if (x[1] + 1 == config.boardSizeX)
                {
                    x[2] = 0;
                }
                else
                {
                    x[2] = x[1] + 1;
                }

                if (y[1] - 1 < 0)
                {
                    y[0] = config.boardSizeY - 1;
                }
                else
                {
                    y[0] = y[1] - 1;
                }

                if (y[1] + 1 == config.boardSizeY)
                {
                    y[2] = 0;
                }
                else
                {
                    y[2] = y[1] + 1;
                }
            }
            else
            {
                //policz bez pbc
                if (x[1] - 1 >= 0)
                {
                    x[0] = x[1] - 1;
                }

                if (x[1] + 1 < config.boardSizeX)
                {
                    x[2] = x[1] + 1;
                }

                if (y[1] - 1 >= 0)
                {
                    y[0] = y[1] - 1;
                }

                if (y[1] + 1 < config.boardSizeY)
                {
                    y[2] = y[1] + 1;
                }
            }


        }

        private void overtake(Cell[,] nextBoard, int x, int y, List<int> Xs, List<int> Ys)
        {
            if (board[x, y].grainID >= 0)
            {
                for (int i = 0; i < Xs.Count; i++)
                {
                    if (Xs[i] >= 0 && Ys[i] >= 0)
                    {
                        if (board[Xs[i], Ys[i]].grainID == -1)//jeżeli komórka nie była zajęta w poprzednim kroku
                        {
                            if (nextBoard[Xs[i], Ys[i]].grainID != -1)//jeżeli komórka jest już zajęta w tym kroku
                            {
                                Random rnd = new Random();
                                if (rnd.Next(0, 1) == 1)//szansa 50%
                                {
                                    nextBoard[Xs[i], Ys[i]].grainID = board[x, y].grainID;
                                }
                            }
                            else
                            {
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

            // X = x;
            //Y = y;
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
        private List<List<int>> prepareHexLeft(List<int> X, List<int> Y)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();

            x.Add(X[0]);
            y.Add(Y[0]);

            x.Add(X[1]);
            y.Add(Y[1]);

            x.Add(X[3]);
            y.Add(Y[3]);

            x.Add(X[4]);
            y.Add(Y[4]);

            x.Add(X[6]);
            y.Add(Y[6]);

            x.Add(X[7]);
            y.Add(Y[7]);

            List<List<int>> tmp = new List<List<int>>();
            tmp.Add(x);
            tmp.Add(y);
            return tmp;
        }
        private List<List<int>> prepareHexRight(List<int> X, List<int> Y)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();

            x.Add(X[1]);
            y.Add(Y[1]);

            x.Add(X[2]);
            y.Add(Y[2]);

            x.Add(X[3]);
            y.Add(Y[3]);

            x.Add(X[4]);
            y.Add(Y[4]);

            x.Add(X[5]);
            y.Add(Y[5]);

            x.Add(X[6]);
            y.Add(Y[6]);

            List<List<int>> tmp = new List<List<int>>();
            tmp.Add(x);
            tmp.Add(y);
            return tmp;
        }
        private List<List<int>> prepareHexRand(List<int> X, List<int> Y)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();

            int number = rnd.Next(0, 2);
            if (number == 1)
            {
                return (prepareHexRight(X, Y));
            }
            else
            {
                return (prepareHexLeft(X, Y));
            }

        }
        private List<List<int>> preparePentRandom(List<int> X, List<int> Y)
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
            
            int number = rnd.Next(0, 4);

            switch (number)
            {
                case 0:
                    x.Add(X[0]);
                    y.Add(Y[0]);
                    break;
                case 1:
                    x.Add(X[2]);
                    y.Add(Y[2]);
                    break;
                case 2:
                    x.Add(X[5]);
                    y.Add(Y[5]);
                    break;
                case 3:
                    x.Add(X[7]);
                    y.Add(Y[7]);
                    break;
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

        public void AddSeed(int x, int y)
        {
            config.grainsCount++;
            grainList.Add(new Grain(config.grainsCount - 1, config.grainsCount));
            int X = x / config.grainXsize;
            int Y = y / config.grainYsize;
            board[X, Y].grainID = config.grainsCount - 1;
        }






    }
}
