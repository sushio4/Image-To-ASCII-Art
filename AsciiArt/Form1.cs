using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;



namespace AsciiArt
{
    public partial class Form1 : Form
    {
        ASCIIMaker maker;
        string text;


        public Form1()
        {
            InitializeComponent();
            hScrollBar1.MouseCaptureChanged += new System.EventHandler(Update);
            hScrollBar2.MouseCaptureChanged += new System.EventHandler(Update);
        }

        private void putText()
        {
            var builder = new StringBuilder();

            for (int y = 0; y < maker.aheight; y++)
            {
                for (int x = 0; x < maker.awidth; x++)
                {
                    builder.Append(maker.ascii[x + y * maker.awidth]);
                }
                builder.Append('\n');
            }

            text = builder.ToString();
            richTextBox1.Text = text;
        }

        private void AdjustFont()
        {
            //richTextBox1.Font = new Font("Consolas", 300 / maker.awidth);
            richTextBox1.ZoomFactor = richTextBox1.Height /(float)(richTextBox1.Font.Height * maker.aheight);
        }

        private void _update()
        {
            
        }
        private void Update(object sender, EventArgs e)
        {
            ThreadStart start = new ThreadStart(maker.MakeGray);
            //start += () =>
            {
            maker.MakeGray();
                maker.AdjustContrast();
                maker.MakeAscii();
                AdjustFont();
                putText();
            };
            Thread tr = new Thread(start);
            //tr.Start();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(openFileDialog1.FileName);
                maker = new ASCIIMaker((Bitmap)img.Clone());

                pictureBox1.Height = pictureBox1.Width * img.Height / img.Width;
                Height = 60 + pictureBox1.Height;
                richTextBox1.Height = pictureBox1.Height;
                pictureBox1.Image = img;

                maker.SetWidth(hScrollBar1.Value);
                
                Update(null,null);
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            maker.SetWidth(hScrollBar1.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            maker.contrastFactor = hScrollBar2.Value / 10;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            maker.invert = checkBox1.Checked;

            Update(null,null);
        }
    }
}
