using System;
using System.Windows.Controls;
using System.Windows.Forms;
using Nostrum;
using Application = System.Windows.Application;

namespace GuitarRigLauncher
{
    public class Tray : IDisposable
    {
        private readonly ContextMenu _contextMenu;
        private readonly NotifyIcon _tray;
        private readonly MenuItem _toggleButton;

        public Tray()
        {
            var icon = MiscUtils.GetEmbeddedIcon("gr.ico");
            _tray = new NotifyIcon
            {
                Icon = icon,
                Visible = true
            };

            _contextMenu = new ContextMenu();
            _contextMenu.Items.Add(new MenuItem { Header = "Change GR5 path...", Command = new RelayCommand(_ => App.BrowseGR()) });
            _toggleButton = new MenuItem { Header = "Unfocusable", IsCheckable = true, IsChecked = true, Command = new RelayCommand(_ =>
            {
                App.Toggle();
                _toggleButton.IsChecked = App.Unfocusable;

            }) };
            _contextMenu.Items.Add(_toggleButton);
            _contextMenu.Items.Add(new MenuItem { Header = "Close", Command = new RelayCommand(_ => Application.Current.Shutdown()) });


            _tray.MouseDown += OnTrayMouseDown;

        }

        private void OnTrayMouseDown(object sender, MouseEventArgs e)
        {
            _contextMenu.IsOpen = e.Button switch
            {
                MouseButtons.Right => true,
                MouseButtons.Left => false,
                _ => _contextMenu.IsOpen
            };
        }

        public void Dispose()
        {
            _tray.Dispose();
        }
    }
}