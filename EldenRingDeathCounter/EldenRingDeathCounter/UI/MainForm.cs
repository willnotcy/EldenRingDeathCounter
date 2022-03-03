using EldenRingDeathCounter.Model;
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
        private readonly LocationDetector locationDetector = new LocationDetector();
        private readonly long minTimeSinceLastDeath = 100_000_000;
        private ContextMenu cm = new ContextMenu();
        private bool running = true;
        private Thread detectionThread;
        private int refreshRate = 500;
        private long lastDeath = 0;

        private ILocation currentLocation;
        private ILocation lastLocation;

        private DebugImageForm debugForm;

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

                if (deathDetector.TryDetectDeath(ScreenGrabber.TakeScreenshot(), out Image<Rgba32> debug, out string debugReading))
                {
                    if (debugForm.Visible)
                    {
                        debugForm.RefreshDeathImage(debug);
                        debugForm.UpdateReading(debugReading);
                    }

                    var now = Stopwatch.GetTimestamp();

                    if (now - lastDeath > minTimeSinceLastDeath)
                    {
                        lastDeath = Stopwatch.GetTimestamp();
                        Console.WriteLine("You died!");
                        IncrementDeathCount();
                    }
                }

                if (locationDetector.TryDetectLocation(ScreenGrabber.TakeScreenshot(), out ILocation location, out Image<Rgba32> debugLocation, out string debugLocationReading))
                {
                    Console.WriteLine($"Detected location: {location}");
                    if (debugForm.Visible)
                    {
                        debugForm.RefreshLocationImage(debugLocation);
                        debugForm.UpdateReading(debugLocationReading);
                    }

                    if (currentLocation is null || currentLocation != location)
                    {
                        if(currentLocation.MultiRegion)
                        {
                            location.Region = currentLocation.Region;
                        }

                        lastLocation = currentLocation;
                        currentLocation = location;

                        UpdateLocation();
                    }
                }

                if (debugForm.Visible)
                {
                    //debugForm.RefreshDeathDebugImage(debug);
                    debugForm.RefreshLocationDebugImage(debugLocation);
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
            var debugModeMenuItem = new MenuItem("debug mode");
            debugModeMenuItem.Click += new System.EventHandler(this.cmsMenuItem_Click);
            cm.MenuItems.Add(debugModeMenuItem);
            this.ContextMenu = cm;

            debugForm = new DebugImageForm();

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
            Screen secondaryScreen = Screen.PrimaryScreen; // Failsafe
            foreach (var screen in Screen.AllScreens)
            {
                if(!screen.Primary)
                {
                    secondaryScreen = screen;
                }
            }

            var formBounds = this.Bounds;

            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = new System.Drawing.Rectangle(secondaryScreen.Bounds.X, secondaryScreen.Bounds.Y, formBounds.Width, formBounds.Height);
        }

        private void SetupHotkeys()
        {
            khm.UnregisterAll();
            // Load prefered keys or defaults:
            int startPauseKeyCode = Settings.Default.StartPause;
            int resetKeyCode = Settings.Default.Reset;

            khm.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control, KeyInterop.VirtualKeyFromKey((Key) startPauseKeyCode), StartPause);
            khm.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control, KeyInterop.VirtualKeyFromKey((Key)resetKeyCode), Reset);
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

        private void UpdateLocation()
        {
            label3.BeginInvoke((MethodInvoker)delegate ()
            {
                label3.Text = $"{currentLocation}";
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

        private void StartPause()
        {
            if (running)
                detectionThread.Abort();
            else
                StartUpdateLoop();

            running = !running;

            button3.BeginInvoke((MethodInvoker)delegate ()
            {
                button3.Text = running ? "Pause" : "Start";
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StartPause();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IncrementDeathCount();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DecrementDeathCount();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RebindPopup rebindPopup = new RebindPopup();
            rebindPopup.ShowDialog(this);
            SetupHotkeys();
        }

        private void cmsMenuItem_Click(object sender, EventArgs e)
        {
            debugForm = new DebugImageForm();
            debugForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}