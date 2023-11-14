using AppUi.Properties;
using System.Diagnostics;

namespace AppUi
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Process? server;

            server = Process.Start(new ProcessStartInfo
            {
                FileName = @$"{Environment.CurrentDirectory}\Kanoe.exe",
                UseShellExecute = false,
                CreateNoWindow = true
            }); ;
            if (server != null)
            {
                Application.Run(new TrayApplicationContext(server));
            }

        }
    }

    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon TrayIcon;
        private readonly Process Server;

        public TrayApplicationContext(Process server)
        {
            Server = server;
            TrayIcon = new NotifyIcon()
            {
                Icon = Resources.Icon,
                ContextMenuStrip = new()
                {
                    Items =
                    {
                        new ToolStripMenuItem("Open", null, new EventHandler(Open), "OPEN"),
                        new ToolStripMenuItem("Exit", null, new EventHandler(Exit), "EXIT")
                    }
                },
                Visible = true
            };
        }

        ~TrayApplicationContext()
        {
            TrayIcon.Visible = false;
            Server.Kill();
            Application.Exit();
        }

        void Open(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://localhost:5026", //TODO: handle non default
                UseShellExecute = true
            });
        }

        void Exit(object? sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            Server.Kill();
            Application.Exit();
        }
    }
}