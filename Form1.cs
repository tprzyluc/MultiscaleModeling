using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Metal
{
    public partial class Form1 : Form
    {

        BitmapGenerator bitmapGenerator;
        SimulationConfig config;
        SimulationEngine engine;
        Random rnd = new Random();


        //bool running;

        public Form1()
        {
            InitializeComponent();
            //comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;


            config = new SimulationConfig();
            config.boardSizeX = Int32.Parse(textBox1.Text);
            config.boardSizeY = Int32.Parse(textBox2.Text);
            pictureBox1.Width = Int32.Parse(textBox1.Text);
            pictureBox1.Height = Int32.Parse(textBox2.Text);
            config.grainsCount = Int32.Parse(textBox3.Text);
            config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
            config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;
            config.minimalGrainsDistance = 0;
            config.probability = Int32.Parse(textBox6.Text);

            config.inclusions = Int32.Parse(textBox4.Text);


            if (!textBox4.ReadOnly)
                config.minimalGrainsDistance = Int32.Parse(textBox4.Text);
            config.simulationType = comboBox3.SelectedIndex;
            config.pbc = false;
            //if(comboBox1.SelectedIndex==0)
            //    config.pbc = true;

            engine = new SimulationEngine(config);

            bitmapGenerator = new BitmapGenerator(config.boardSizeX, config.boardSizeY, pictureBox1.Size.Width, pictureBox1.Size.Height);
            if (checkBox2.Checked)
            {
                for (int i = 0; i < config.inclusions; i++)
                {
                    int x = rnd.Next(0, config.boardSizeX);
                    int y = rnd.Next(0, config.boardSizeY);


                    engine.board[x, y].grainID = -2;
                    if (comboBox1.SelectedIndex == 0)
                    {

                        for (int j = 0; j < Int32.Parse(textBox5.Text); j++)
                        {
                            for (int k = 0; k < Int32.Parse(textBox5.Text); k++)
                            {
                                if (x + j < config.boardSizeX && y + k < config.boardSizeY)
                                    engine.board[x + j, y + k].grainID = -2;
                            }
                        }
                    }

                    if (comboBox1.SelectedIndex == 1)
                    {
                        double r, rr;
                        r = Int32.Parse(textBox5.Text);
                        rr = Math.Pow(r, 2);
                        for (int j = x - (int)r; j <= x + r; j++)
                            for (int k = y - (int)r; k <= y + r; k++)
                                if (Math.Abs(Math.Pow(j - x, 2) + Math.Pow(k - y, 2)) <= rr)
                                    if (j < config.boardSizeX && j >= 0 && k < config.boardSizeY && k >= 0)
                                        engine.board[j, k].grainID = -2;

                    }


                }



            }
            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);

            pictureBox1.Image = bitmapGenerator.bmp;



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            engine.GenerateNextStep();
            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);

            pictureBox1.Image = bitmapGenerator.bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            engine.GenerateNextStep();
            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);

            if (engine.CountEmptyCells() == 0)
            {


                timer1.Enabled = false;

                Color tmp;
                tmp = Color.White;
                List<int> xcords = new List<int>();
                List<int> ycords = new List<int>();

                if (checkBox3.Checked)
                {
                    for (int i = 0; i < config.boardSizeX; i++)
                    {
                        for (int j = 0; j < config.boardSizeY; j++)
                        {
                            if (tmp != bitmapGenerator.bmp.GetPixel(i, j))
                            {
                                tmp = bitmapGenerator.bmp.GetPixel(i, j);
                                xcords.Add(i);
                                ycords.Add(j);

                            }
                        }
                    }

                   

                    for (int i = 0; i < config.inclusions; i++)
                    {
                        int xs = rnd.Next(0, xcords.Count);


                        int x = xcords[xs];
                        int y = ycords[xs];

                        engine.board[x, y].grainID = -2;

                        if (comboBox1.SelectedIndex == 0)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                for (int k = 0; k < 5; k++)
                                {
                                    if (x + j < config.boardSizeX && y + k < config.boardSizeY)
                                        engine.board[x + j, y + k].grainID = -2;
                                }
                            }
                        }

                        if (comboBox1.SelectedIndex == 1)
                        {
                            double r, rr;
                            r = Int32.Parse(textBox5.Text);
                            rr = Math.Pow(r, 2);
                            for (int j = x - (int)r; j <= x + r; j++)
                                for (int k = y - (int)r; k <= y + r; k++)
                                    if (Math.Abs(Math.Pow(j - x, 2) + Math.Pow(k - y, 2)) <= rr)
                                        if (j < config.boardSizeX && j >= 0 && k < config.boardSizeY && k >= 0)
                                            engine.board[j, k].grainID = -2;
                        }



                    }
                    bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);
                }

                pictureBox1.Image = bitmapGenerator.bmp;

            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Cell[,] tmp_board = new Cell[config.boardSizeX, config.boardSizeY];


            if (!timer1.Enabled)
            {
                if (checkBox1.Checked)
                {
                    engine.selected_grain_ids.Add(engine.board[e.X, e.Y].grainID);



                    //for (int i = 0; i < config.boardSizeX; i++)
                    //{
                    //    for (int j = 0; j < config.boardSizeY; j++)
                    //    {
                    //        tmp_board[i, j] = new Cell();

                    //    }
                    //}

                    //tmp_board = engine.board;



                    //for (int i = 0; i < config.boardSizeX; i++)
                    //{
                    //    for (int j = 0; j < config.boardSizeY; j++)
                    //    {
                    //        Console.WriteLine("X " + i + " Y " + i + " GrainID "+ tmp_board[i, j].grainID);

                    //    }
                    //}
                    engine.board[e.X, e.Y].selected = true;
                    for (int i = 0; i < config.boardSizeX; i++)
                    {
                        for (int j = 0; j < config.boardSizeY; j++)
                        {
                            if (engine.selected_grain_ids.Contains(engine.board[i, j].grainID))
                            {
                                engine.board[i, j].selected = true;
                                //Console.WriteLine(engine.selected_grain_ids.Contains(engine.board[i, j].grainID));
                            }
                        }
                    }
                    tmp_board = engine.board;
                    for (int i = 0; i < config.boardSizeX; i++)
                    {
                        for (int j = 0; j < config.boardSizeY; j++)
                        {
                            if (engine.selected_grain_ids.Contains(engine.board[i, j].grainID))
                            {
                                engine.board[i, j].selected = true;
                                //Console.WriteLine(engine.selected_grain_ids.Contains(engine.board[i, j].grainID));
                            }
                        }
                    }











                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void openFileDialog2_FileOk_1(object sender, CancelEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "bmp or txt |*.bmp; *.json";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = System.IO.Path.GetExtension(dlg.FileName);
                    switch (ext)
                    {
                        case ".json":

                            using (StreamReader r = new StreamReader(dlg.FileName))
                            {
                                string json = r.ReadToEnd();
                                SimulationEngine simulationEngine = JsonConvert.DeserializeObject<SimulationEngine>(json);
                                Console.WriteLine(simulationEngine.config.boardSizeX);

                                config = simulationEngine.config;
                                timer1.Enabled = false;

                                pictureBox1.Width = config.boardSizeX;
                                pictureBox1.Height = config.boardSizeY;
                                config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
                                config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;
                                config.minimalGrainsDistance = 0;

                                engine = simulationEngine;

                                bitmapGenerator = new BitmapGenerator(config.boardSizeX, config.boardSizeY, pictureBox1.Size.Width, pictureBox1.Size.Height);

                                bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);

                                pictureBox1.Image = bitmapGenerator.bmp;


                            }

                            break;


                        case ".bmp":
                            Bitmap loadedBitmap = new Bitmap(dlg.FileName);
                            int color_counter = 0;
                            config = new SimulationConfig();
                            config.boardSizeX = loadedBitmap.Width;
                            config.boardSizeY = loadedBitmap.Height;
                            pictureBox1.Width = loadedBitmap.Width;
                            pictureBox1.Height = loadedBitmap.Height;
                            config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
                            config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;

                            Console.WriteLine(config.boardSizeX);
                            Console.WriteLine(config.boardSizeY);

                            List<Color> colorList = new List<Color>();
                            List<int> coord_X = new List<int>();
                            List<int> coord_Y = new List<int>();




                            engine = new SimulationEngine(config);


                            for (int i = 0; i < config.boardSizeX; i++)
                            {
                                for (int j = 0; j < config.boardSizeY; j++)
                                {
                                    colorList.Add(loadedBitmap.GetPixel(i, j));
                                    coord_X.Add(j);
                                    coord_Y.Add(i);

                                }
                            }

                            colorList = colorList.Distinct().ToList<Color>();



                            Console.WriteLine(colorList.Count);



                            color_counter = (from x in colorList
                                             select x).Distinct().Count();
                            color_counter -= 1; // because of white color

                            Console.WriteLine(color_counter);

                            List<Grain> grainList_loaded = new List<Grain>();



                            config.grainsCount = color_counter;
                            config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
                            config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;

                            config.simulationType = 1;



                            engine = new SimulationEngine(config, true);
                            engine.grainList = grainList_loaded;
                            for (int i = 0; i < config.grainsCount; i++)
                            {
                                grainList_loaded.Add(new Grain(i, config.grainsCount, colorList[i], coord_X[i], coord_Y[i]));
                                engine.board[coord_X[i], coord_Y[i]].grainID = grainList_loaded[i].ID;
                                Console.WriteLine("X " + coord_X[i] + "Y" + coord_Y[i] + "Color " + colorList[i]);
                            }

                            bitmapGenerator = new BitmapGenerator(config.boardSizeX, config.boardSizeY, pictureBox1.Size.Width, pictureBox1.Size.Height);

                            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);


                            pictureBox1.Image = loadedBitmap;


                            break;


                    }


                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "bmp, json|*.bmp;*.json";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".json":
                        JObject config_to_save = new JObject(
                                new JProperty("boardSizeX", config.boardSizeX),
                                new JProperty("boardSizeY", config.boardSizeY),
                                new JProperty("grainXsize", config.grainXsize),
                                new JProperty("grainYsize", config.grainYsize),
                                new JProperty("grainsCount", config.grainsCount)
                      );
                        //using (StreamWriter file = File.CreateText(path))
                        //using (JsonTextWriter config_file = new JsonTextWriter(file))
                        //{
                        //    config_to_save.WriteTo(config_file);
                        //}
                        File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(engine, Formatting.Indented));
                        break;

                    case ".bmp":
                        bitmapGenerator.bmp.Save(sfd.FileName);

                        break;

                }
                // pictureBox1.Image.Save(sfd.FileName, format);
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {


            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    if (!engine.board[i, j].selected)
                    {
                        if (engine.board[i, j].grainID == -2)
                            continue;
                        engine.board[i, j].grainID = -1;
                    }
                }
            }
            bitmapGenerator.CA_board(engine.board);
            pictureBox1.Image = bitmapGenerator.bmp;


        }



        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    if (!engine.board[i, j].selected)
                    {
                        if (engine.board[i, j].grainID == -2)
                            continue;
                        engine.board[i, j].grainID = -1;
                    }
                    else
                    {
                        engine.board[i, j].grainID = -4;
                    }
                }
            }
            bitmapGenerator.CA_board_v2(engine.board);
            pictureBox1.Image = bitmapGenerator.bmp;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            config.grainsCount = Int32.Parse(textBox3.Text);
            config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
            config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;

            engine.GenerateRandomSeeds_v2();

            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);
            pictureBox1.Image = bitmapGenerator.bmp;




        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    if (!engine.board[i, j].selected)
                    {
                        if (engine.board[i, j].grainID == -2)
                            continue;
                        engine.board[i, j].grainID = -1;
                    }
                }
            }
            bitmapGenerator.CA_board(engine.board);
            pictureBox1.Image = bitmapGenerator.bmp;



            Color tmp;
            tmp = Color.White;
            List<int> xcords = new List<int>();
            List<int> ycords = new List<int>();


            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    if (tmp != bitmapGenerator.bmp.GetPixel(i, j))
                    {
                        tmp = bitmapGenerator.bmp.GetPixel(i, j);
                        xcords.Add(i);
                        ycords.Add(j);

                    }
                    else
                    {
                        if(i-1>0)
                            if(tmp != bitmapGenerator.bmp.GetPixel(i-1, j))
                            {
                                xcords.Add(i-1);
                                ycords.Add(j);
                            }

                        if (i + 1  < config.boardSizeX)
                            if (tmp != bitmapGenerator.bmp.GetPixel(i + 1, j))
                            {
                                xcords.Add(i + 1);
                                ycords.Add(j);
                            }


                    }
                }
            }





            for (int i = 0; i < xcords.Count; i++)
            {



                int x = xcords[i];
                int y = ycords[i];

                engine.board[x, y].grainID = -2;

                for (int j = 0; j < Int32.Parse(textBox5.Text); j++)
                {
                    for (int k = 0; k < Int32.Parse(textBox5.Text); k++)
                    {
                        if (x + j < config.boardSizeX && y + k < config.boardSizeY)
                            engine.board[x + j, y + k].grainID = -2;
                    }
                }

            }

            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {

                    if (engine.board[i, j].grainID == -2)
                        continue;
                    engine.board[i, j].grainID = -1;

                }
            }


            bitmapGenerator.CA_board(engine.board);
                    pictureBox1.Image = bitmapGenerator.bmp;



        }

        private void button10_Click(object sender, EventArgs e)
        {
            Color tmp;
            tmp = Color.White;
            List<int> xcords = new List<int>();
            List<int> ycords = new List<int>();


            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {
                    if (tmp != bitmapGenerator.bmp.GetPixel(i, j))
                    {
                        tmp = bitmapGenerator.bmp.GetPixel(i, j);
                        xcords.Add(i);
                        ycords.Add(j);

                    }
                    else
                    {
                        if (i - 1 > 0)
                            if (tmp != bitmapGenerator.bmp.GetPixel(i - 1, j))
                            {
                                xcords.Add(i - 1);
                                ycords.Add(j);
                            }

                        if (i + 1 < config.boardSizeX)
                            if (tmp != bitmapGenerator.bmp.GetPixel(i + 1, j))
                            {
                                xcords.Add(i + 1);
                                ycords.Add(j);
                            }


                    }
                }
            }





            for (int i = 0; i < xcords.Count; i++)
            {



                int x = xcords[i];
                int y = ycords[i];

                engine.board[x, y].grainID = -2;

                for (int j = 0; j < Int32.Parse(textBox5.Text); j++)
                {
                    for (int k = 0; k < Int32.Parse(textBox5.Text); k++)
                    {
                        if (x + j < config.boardSizeX && y + k < config.boardSizeY)
                            engine.board[x + j, y + k].grainID = -2;
                    }
                }

            }

            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {

                    if (engine.board[i, j].grainID == -2)
                        continue;
                    engine.board[i, j].grainID = -1;

                }
            }


            bitmapGenerator.CA_board(engine.board);
            pictureBox1.Image = bitmapGenerator.bmp;

            int boundary_counter=0;

            for (int i = 0; i < config.boardSizeX; i++)
            {
                for (int j = 0; j < config.boardSizeY; j++)
                {

                    if (engine.board[i, j].grainID == -2)
                        boundary_counter++;

                }
            }
            boundary_counter = (boundary_counter*100 / (config.boardSizeX * config.boardSizeY));
            label8.Text = boundary_counter.ToString() + "%";


        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}


