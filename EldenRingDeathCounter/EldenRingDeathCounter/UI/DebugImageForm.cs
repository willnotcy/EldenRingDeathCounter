using EldenRingDeathCounter.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EldenRingDeathCounter
{
    public partial class DebugImageForm : Form
    {
        private readonly DeathDetector deathDetector = new DeathDetector();
        private static float threshold = 0.24f;


        public DebugImageForm()
        {
            InitializeComponent();
        }
        public void RefreshImage(Image<Rgba32> bmp)
        {
            var stream = new System.IO.MemoryStream();
            bmp.SaveAsBmp(stream);
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            UpdateForm(img);
        }

        public void UpdateForm(System.Drawing.Image img)
        {
            pictureBox1.Image = img;

            textBox1.Invoke((MethodInvoker)delegate ()
            {
               textBox1.Text = threshold.ToString();
            });
        }

        public void Refresh()
        {
            var yes = deathDetector.TryDetectDeath(ScreenGrabber.TakeScreenshot(), out Image<Rgba32> debug, out string debugReading);

            var stream = new System.IO.MemoryStream();
            debug.SaveAsBmp(stream);
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            UpdateForm(img);
            UpdateReading(debugReading);
        }

        public void UpdateReading(string reading)
        {
            textBox3.Invoke((MethodInvoker)delegate ()
            {
                textBox3.Text = reading.ToString();
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            threshold = threshold - 0.005f;
            textBox1.Text = threshold.ToString();
            Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            threshold = threshold + 0.005f;
            textBox1.Text = threshold.ToString();
            Refresh();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
