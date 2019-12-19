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

        public Random random = new Random();





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
            bmp = new Bitmap(bitmapX, bitmapY);

            for (int i = 0; i < boardX; i++)
            {
                for (int j = 0; j < boardY; j++)
                {
                    if (board[i, j].grainID == -1)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.White);

                    }
                    else if (board[i, j].grainID == -2)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.Black);


                    }
                    else if (board[i, j].grainID == -4)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.Blue);

                    }
                    else
                    {
                        DrawCell(i * grainXsize, j * grainYsize, grainList.ElementAt(board[i, j].grainID).color);
                        grainList.ElementAt(board[i, j].grainID).posX = i * grainXsize;
                        grainList.ElementAt(board[i, j].grainID).posY = j * grainYsize;

                    }
                }
            }
        }

        public void DrawEnergy(Cell[,] board)
        {
            
            bmp = new Bitmap(bitmapX, bitmapY);
            
            for (int i = 0; i < boardX; i++)
                for (int j = 0; j < boardY; j++)
                {
                    if(board[i,j].boundary)
                        DrawCell(i * grainXsize, j * grainYsize, Color.FromArgb((int)board[i, j].energy,0,0));
                    else
                        DrawCell(i * grainXsize, j * grainYsize, Color.FromArgb(200 - (int)board[i, j].energy, 0, (int)board[i, j].energy));

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

        public void CA_board(Cell[,] board)
        {
            for (int i = 0; i < boardX; i++)
            {
                for (int j = 0; j < boardY; j++)
                {
                    if (board[i, j].grainID == -2)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.Black);


                    }
                    else if (board[i, j].grainID == -1)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.White);

                    }        
                }
            }
        }

        public void CA_board_v2(Cell[,] board)
        {
            for (int i = 0; i < boardX; i++)
            {
                for (int j = 0; j < boardY; j++)
                {
                    if (board[i, j].grainID == -2)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.Black);


                    }
                    else if (board[i, j].grainID == -1)
                    {
                        DrawCell(i * grainXsize, j * grainYsize, Color.White);

                    }
                    else if (board[i,j].selected)
                        DrawCell(i * grainXsize, j * grainYsize, Color.HotPink);
                }
            }
        }
    }







}

