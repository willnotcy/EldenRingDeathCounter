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
        private readonly LocationHelper locationHelper = LocationHelper.Instance;
        private readonly BossDetector bossDetector = new BossDetector();
        private readonly BossCounter bossCounter = BossCounter.Instance;
        private readonly long minTimeSinceLastDeath = 100_000_000;
        private readonly long maxTimeSinceBoss = 200_000_000;
        private ContextMenu cm = new ContextMenu();
        private bool running = true;
        private Thread detectionThread;
        private int refreshRate = 400;
        private long lastDeath = 0;
        private long lastBossTs = 0;

        private ILocation currentLocation;
        private ILocation lastLocation;
        private IBoss currentBoss;
        private IBoss lastBoss;

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

            if(currentLocation is not null)
            {
                UpdateLocation();
            }
 
            while (true)
            {
                sw.Restart();

                var sc = ScreenGrabber.TakeScreenshot();

                DeathDetection(sc.Clone(), out Image<Rgba32> debugDeath);

                LocationDetection(sc.Clone(), out Image<Rgba32> debugLocation);

                BossDetection(sc.Clone(), out Image<Rgba32> debugBoss);

                sc.Dispose();

                if (debugForm.Visible)
                {
                    //debugForm.RefreshDeathDebugImage(debugDeath);
                    debugForm.RefreshLocationDebugImage(debugLocation);
                }

                sw.Stop();
                long elapsedTime = sw.ElapsedMilliseconds;

                if (elapsedTime < refreshRate)
                {
                    Thread.Sleep(refreshRate - (int)elapsedTime);
                }
            }
        }

        private void BossDetection(Image<Rgba32> sc, out Image<Rgba32> debugBoss)
        {
            var now = Stopwatch.GetTimestamp();
            if (bossDetector.TryDetectBoss(sc, currentLocation, out IBoss boss, out debugBoss, out string debugBossReading))
            {
                Console.WriteLine($"Detected boss: {boss}");
                lastBossTs = now;

                if (debugForm.Visible)
                {
                    debugForm.RefreshLocationImage(debugBoss);
                    debugForm.UpdateReading("Detected boss: {boss}");
                }

                if(currentBoss is null || currentBoss != boss)
                {
                    lastBoss = currentBoss;
                    currentBoss = boss;
                    UpdateCount();
                }
                UpdateBoss();
            } else
            {
                if (now - lastBossTs > maxTimeSinceBoss)
                {
                    if(currentBoss is not null)
                    {
                        currentBoss = null;
                        Reset();
                        UpdateBoss();
                    }
                }
            }
        }

        private void LocationDetection(Image<Rgba32> sc, out Image<Rgba32> debugLocation)
        {
            if (locationDetector.TryDetectLocation(sc, currentLocation, out ILocation location, out debugLocation, out string debugLocationReading))
            {
                Console.WriteLine($"Detected location: {location}");
                if (debugForm.Visible)
                {
                    debugForm.RefreshLocationImage(debugLocation);
                    debugForm.UpdateReading(debugLocationReading);
                }

                if (currentLocation is null || currentLocation != location)
                {
                    lastLocation = currentLocation;
                    currentLocation = location;

                    UpdateLocation();
                }
            }
        }

        private void DeathDetection(Image<Rgba32> sc, out Image<Rgba32> debugDeath)
        {
            if (deathDetector.TryDetectDeath(sc, out debugDeath, out string debugReading))
            {
                if (debugForm.Visible)
                {
                    debugForm.RefreshDeathImage(debugDeath);
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

            LoadLocation();
        }
        private void SetName()
        {
            this.Name = "Karc's Elden Ring Death Counter";
            this.Text = "Karc's Elden Ring Death Counter";
        }

        private void LoadLocation()
        {
            string lastLocationKey = Settings.Default.LastKnownLocationKey;
            string lastRegionName = Settings.Default.LastKnownRegionName;
;

            if(lastLocationKey is null || lastLocationKey.Equals(""))
            {
                return;
            }

            if(lastRegionName is not null && !lastRegionName.Equals(""))
            {
                var region = locationHelper.GetRegion(lastRegionName);
                if (locationHelper.TryGetLocation(lastLocationKey, region.Locations.First(), out ILocation location))
                {
                    currentLocation = location;
                }
            } else
            {
                if (locationHelper.TryGetLocation(lastLocationKey, null, out ILocation location))
                {
                    currentLocation = location;
                }
            }
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

            if(currentBoss != null)
            {
                bossCounter.TryIncrementCount(currentBoss);
            }

            UpdateCount();
        }

        private void DecrementDeathCount()
        {
            if (DeathCount < 1)
                return;

            DeathCount--;

            if (currentBoss != null)
            {
                bossCounter.TryDecrementCount(currentBoss);
            }

            UpdateCount();
        }

        private void UpdateCount()
        {
            if(currentBoss != null)
            {
                if (bossCounter.TryGetCount(currentBoss, out int count))
                {
                    DeathCount = count;
                }
            }

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

            Settings.Default.LastKnownLocationKey = currentLocation.Name.ToLower().Replace(" ", "").Trim();
            Settings.Default.LastKnownRegionName = currentLocation.Region.ParentRegion is null ? currentLocation.Region.Name : currentLocation.Region.ParentRegion.Name;
            Settings.Default.Save();
        }

        private void UpdateBoss()
        {
            label4.BeginInvoke((MethodInvoker)delegate ()
            {
                label4.Text = currentBoss is null ? "" : $"{currentBoss.Name}";
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

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}