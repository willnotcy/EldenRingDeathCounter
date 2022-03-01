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
            this.textBox1.Text = CharFromKeyCode(Settings.Default.StartPause).ToString();
            this.textBox2.Text = CharFromKeyCode(Settings.Default.Reset).ToString();
        }

        private Char CharFromKeyCode(int KeyCode)
        {
            return (char)KeyInterop.VirtualKeyFromKey((Key) KeyCode);
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
                DisplayError("Can't bind the same key to multiple functions...");
                return;
            }

            int startPauseKeyCode = (int)Enum.Parse(typeof(Key), txt1);
            int resetKeyCode = (int)Enum.Parse(typeof(Key), txt2);

            Settings.Default.StartPause = startPauseKeyCode;
            Settings.Default.Reset = resetKeyCode;
            Settings.Default.Save();
            DisplayMessage("Updated keybinds");

            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            HandleTextBoxTextChanged((TextBox)sender);
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
            HandleTextBoxTextChanged((TextBox)sender);
        }

        private void HandleTextBoxTextChanged(TextBox txtBox)
        {
            var txt = txtBox.Text.ToUpper();

            if (txt.Length == 0)
                return;

            txtBox.Text = txt;
        }
    }
}
