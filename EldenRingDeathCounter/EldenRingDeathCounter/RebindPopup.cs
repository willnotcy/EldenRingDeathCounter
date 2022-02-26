using EldenRingDeathCounter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace EldenRingDeathCounter
{
    public partial class RebindPopup : Form
    {
        public RebindPopup()
        {
            InitializeComponent();
        }

        private void RebindPopup_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = CharFromKeyCode(Settings.Default.IncrementKeyCode).ToString();
            this.textBox2.Text = CharFromKeyCode(Settings.Default.DecrementKeyCode).ToString();
        }

        private Char CharFromKeyCode(int KeyCode)
        {
            return (Char)KeyInterop.VirtualKeyFromKey((Key) KeyCode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var txt1 = textBox1.Text;
            var txt2 = textBox2.Text;

            if (txt1.Length != 1 || txt2.Length != 1)
            {
                DisplayError("Those are not valid keys...");
                return;
            }

            if (txt1.Equals(txt2))
            {
                DisplayError("Can't bind the same key to both functions...");
                return;
            }
  
            int incrementKeyCode = (int)Enum.Parse(typeof(Key), txt1);
            int decrementKeyCode = (int)Enum.Parse(typeof(Key), txt2);

            Settings.Default.IncrementKeyCode = incrementKeyCode;
            Settings.Default.DecrementKeyCode = decrementKeyCode;
            Settings.Default.Save();
            DisplayMessage("Updated keybinds");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var txt = textBox1.Text.ToUpper();

            if (txt.Length == 0)
                return;

            textBox1.Text = txt;
        }

        private void DisplayError(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DisplayMessage(string msg)
        {
            MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            var txt = textBox2.Text.ToUpper();

            if (txt.Length == 0)
                return;

            textBox2.Text = txt;
        }
    }
}
