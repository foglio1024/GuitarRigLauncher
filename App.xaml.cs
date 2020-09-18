using Nostrum.Extensions;
using Nostrum.WinAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace GuitarRigLauncher
{
    public partial class App : Application
    {
        const string GUITAR_RIG_WINDOW_NAME = "Guitar Rig 5 - Native Instruments";
        public static bool Unfocusable = true;
        private static Process? _guitarRig;
        private static Tray _tray = null!;
        private static Settings _settings = null!;
        private static string SettingsPath => Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location)!, "settings.json");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _tray = new Tray();
            try
            {
                _settings = Settings.LoadFrom(SettingsPath);

            }
            catch (FileNotFoundException)
            {
                _settings = new Settings();
                _settings.Save(SettingsPath);
            }
            Launch();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_guitarRig != null && _guitarRig.HasExited == false)
            {
                _guitarRig.Close();
            }
            Dispatcher.Invoke(() => _tray.Dispose());
        }

        public static void Toggle()
        {
            Unfocusable = !Unfocusable;
            if (_guitarRig == null) return; //todo: raise warning
            var windows = _guitarRig.GetProcessWindows();

            foreach (var win in windows)
            {
                if (Utils.GetWindowTitle(win) != GUITAR_RIG_WINDOW_NAME) continue;
                if (Unfocusable)
                {
                    Utils.MakeWindowUnfocusable(win);
                }
                else
                {
                    Utils.MakeWindowFocusable(win);
                }
            }
        }

        private void Launch()
        {
            if (!File.Exists(_settings.GuitarRigPath))
            {
                BrowseGR();
                Launch();
                return;
            }
            var psi = new ProcessStartInfo(_settings.GuitarRigPath);
            _guitarRig = Process.Start(psi);
            if (_guitarRig == null) return; //todo: raise warning
            _guitarRig.EnableRaisingEvents = true;
            _guitarRig.Exited += OnGuitarRigExited;

            var windows = _guitarRig.GetProcessWindows().ToList();
            while (true)
            {
                if (windows.Any(win => Utils.GetWindowTitle(win) == GUITAR_RIG_WINDOW_NAME)) break;
                windows = _guitarRig.GetProcessWindows().ToList();
                Thread.Sleep(100);
            }

            foreach (var win in windows)
            {
                if (Utils.GetWindowTitle(win) == GUITAR_RIG_WINDOW_NAME) Utils.MakeWindowUnfocusable(win);
            }
        }

        private void OnGuitarRigExited(object? sender, EventArgs e)
        {
            if (_guitarRig != null)
            {
                _guitarRig.Exited -= OnGuitarRigExited;
            }
            Dispatcher.Invoke(() => Current.Shutdown());
        }

        public static void BrowseGR()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Guitar Rig 5.exe location",
                Filter = "Guitar Rig 5 executable | Guitar Rig 5.exe"
            };
            if (dlg.ShowDialog() == DialogResult.OK && File.Exists(dlg.FileName))
            {
                _settings.GuitarRigPath = dlg.FileName;
                _settings.Save(SettingsPath);
            }
            else
            {
                //todo: warn the user
            }
        }
    }
}
