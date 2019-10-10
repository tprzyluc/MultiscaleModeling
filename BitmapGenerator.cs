using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metal
{
    class BitmapGenerator
    {
        public int bitmapX;
        public int bitmapY;
        public int boardX;
        public int boardY;

        public int grainXsize;
        public int grainYsize;
        //public int grainNumber;
        //public List<Grain> grainList;
        //public Cell[,] board;

        public Bitmap bmp;

        public BitmapGenerator(int x, int y, int width, int height)
        {
            bitmapX = width;
            bitmapY = height;
            boardX = x;
            boardY = y;
            grainXsize = bitmapX / boardX;
            grainYsize = bitmapY / boardY;
            
        }


        public void GenerateBitmap(Cell[,] board, List<Grain> grainList)
        {
            //this.board = b;
            bmp = new Bitmap(bitmapX, bitmapY);

            Console.WriteLine("TEST");
            for (int i = 0; i < boardX; i++)
            {
                for (int j = 0; j < boardY; j++)
                {
                    if (board[i, j].grainID == -1)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.White);
                    }
                    else
                    {
                        Color tmp = grainList.ElementAt(board[i, j].grainID).color;
                        DrawCell(i * grainXsize, j * grainYsize, grainList.ElementAt(board[i, j].grainID).color);
                    }
                }
            }
        }

        private void DrawCell(int x, int y, Color color)
        {
            //Console.WriteLine
            for (int i = x; i < x + grainXsize; i++)
            {
                for (int j = y; j < y + grainYsize; j++)
                {
                    bmp.SetPixel(i, j, color);
                }
            }
        }




    }
}
