using EldenRingDeathCounter.Properties;
using NonInvasiveKeyboardHookLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace EldenRingDeathCounter
{
    public partial class MainForm : Form
    {
        private readonly KeyboardHookManager khm = new KeyboardHookManager();

        private int DeathCount { get; set; }

        public MainForm()
        {        
            InitializeComponent();
            SetupComponents();
        }

        private void SetupComponents()
        {
            khm.Start();
            SetupHotkeys();
            SetName();
        } 

        private void SetName()
        {
            this.Name = "Karc's Elden Ring Death Counter";
            this.Text = "Karc's Elden Ring Death Counter";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void SetupHotkeys()
        {
            khm.UnregisterAll();
            // Load prefered keys or defaults:
            int incrementKeyCode = Settings.Default.IncrementKeyCode;
            int decrementKeyCode = Settings.Default.DecrementKeyCode;

            khm.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control, KeyInterop.VirtualKeyFromKey((Key) decrementKeyCode), DecrementDeathCount);
            khm.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control, KeyInterop.VirtualKeyFromKey((Key) incrementKeyCode), IncrementDeathCount);
        }

        private void IncrementDeathCount()
        {
            DeathCount++;
            UpdateCount();
        }

        private void DecrementDeathCount()
        {
            if (DeathCount < 1)
                return;

            DeathCount--;
            UpdateCount();
        }

        private void UpdateCount()
        {
            label2.BeginInvoke((MethodInvoker)delegate ()
            {
                label2.Text = DeathCount.ToString();
            });
        }

        private void Reset()
        {
            DeathCount = 0;
            label2.BeginInvoke((MethodInvoker)delegate ()
            {
                label2.Text = DeathCount.ToString();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var rebindPopup = new RebindPopup();
            rebindPopup.ShowDialog(this);
            SetupHotkeys();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncrementDeathCount();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DecrementDeathCount();
        }
    }
}
