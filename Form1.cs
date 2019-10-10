using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        //bool running;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GENEROWANIE PLANSZY
            timer1.Enabled = false;

            //Wstawiam wartości do konfiguracji dla symulacji
            config = new SimulationConfig();
            config.boardSizeX = Int32.Parse(textBox1.Text);
            config.boardSizeY = Int32.Parse(textBox2.Text);
            config.grainsCount = Int32.Parse(textBox3.Text);
            config.grainXsize = pictureBox1.Size.Width / config.boardSizeX;
            config.grainYsize = pictureBox1.Size.Height / config.boardSizeY;
            config.minimalGrainsDistance = 0;
            if(!textBox4.ReadOnly)
                config.minimalGrainsDistance = Int32.Parse(textBox4.Text);
            config.simulationType = comboBox3.SelectedIndex;
            config.seedPlacement = comboBox2.SelectedIndex;
            config.pbc = false;
            if(comboBox1.SelectedIndex==0)
                config.pbc = true;

            //Inicjalizuje silnik symulacji
            engine = new SimulationEngine(config);          //chyba wszystko

            //Inicjalizuj generator bitmap
            bitmapGenerator = new BitmapGenerator(config.boardSizeX, config.boardSizeY, pictureBox1.Size.Width, pictureBox1.Size.Height);

            //Narysuj przestrzeń
            bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);

            pictureBox1.Image = bitmapGenerator.bmp;

            

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 2)
            {
                textBox4.ReadOnly = false;
            }
            else
            {
                textBox4.ReadOnly = true;
            }
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
                pictureBox1.Image = bitmapGenerator.bmp;
                if(engine.CountEmptyCells() == 0)
                {
                    timer1.Enabled = false;

                }
            

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
            {
                if (checkBox1.Checked)
                {
                    
                    engine.AddSeed(e.X, e.Y);
                    bitmapGenerator.GenerateBitmap(engine.board, engine.grainList);
                    pictureBox1.Image = bitmapGenerator.bmp;
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
            openFileDialog1.InitialDirectory = "C:/";
            openFileDialog1.ShowDialog();
        }
    }
}
