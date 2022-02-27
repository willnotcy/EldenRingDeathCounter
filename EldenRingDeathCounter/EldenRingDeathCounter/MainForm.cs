using EldenRingDeathCounter.Properties;
using EldenRingDeathCounter.Util;
using NonInvasiveKeyboardHookLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace EldenRingDeathCounter
{
    public partial class MainForm : Form
    {
        private readonly KeyboardHookManager khm = new KeyboardHookManager();
        private readonly DeathDetector deathDetector = new DeathDetector();
        private readonly long minTimeSinceLastDeath = 100_000_000;
        private bool running = true;
        private Thread detectionThread;
        private int refreshRate = 200;
        private long lastDeath = 0;


        private int DeathCount { get; set; }

        public MainForm()
        {        
            InitializeComponent();
            SetupComponents();
            StartUpdateLoop();
        }

        private Thread StartUpdateLoop()
        {
            detectionThread = new Thread(UpdateLoop);
            detectionThread.Start();
            return detectionThread;
        }

        private void UpdateLoop()
        {
            Stopwatch sw = new Stopwatch();

            int i = 0;
            while(true)
            {
                sw.Restart();

                if(deathDetector.TryDetectDeath(ScreenGrabber.TakeScreenshot(), out bool dead, out Image<Rgba32> debug))
                {
                    if (dead)
                    {
                        var now = Stopwatch.GetTimestamp();

                        if (now - lastDeath > minTimeSinceLastDeath)
                        {
                            lastDeath = Stopwatch.GetTimestamp();
                            Console.WriteLine("You died!");
                            IncrementDeathCount();
                        }
                    }
                }

                sw.Stop();
                long elapsedTime = sw.ElapsedMilliseconds;

                if(elapsedTime < refreshRate)
                {
                    Thread.Sleep(refreshRate - (int)elapsedTime);
                }
            }
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
            if (running)
                detectionThread.Abort();
            else
                StartUpdateLoop();

            running = !running;

            button3.Text = running ? "Pause" : "Start";
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
