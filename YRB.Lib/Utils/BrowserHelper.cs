using System.Diagnostics;

namespace YRB.Lib.Utils
{
    public static class BrowserHelper
    {
        public static string? GetChromeMainWindowTitle()
        {
            var processes = Process.GetProcesses();
            var chromes = processes.Where(p => p.ProcessName.ToLower().Contains("chrome") && !string.IsNullOrEmpty(p.MainWindowTitle)).ToArray();
            var chrome = chromes.FirstOrDefault();
            return chrome?.MainWindowTitle;
        }

        public static string? GetChromePath()
        {
            var processes = Process.GetProcesses();
            var chromes = processes.Where(p => p.ProcessName.ToLower().Contains("chrome") && !string.IsNullOrEmpty(p.MainWindowTitle)).ToArray();
            var chrome = chromes.FirstOrDefault();
            return chrome?.MainModule?.FileName;
        }

        public static void KillChromes()
        {
            var processes = Process.GetProcesses();
            var chromes = processes.Where(p => p.ProcessName.ToLower().Contains("chrome") && !string.IsNullOrEmpty(p.MainWindowTitle)).ToArray();
            var chrome = chromes.FirstOrDefault();
            chrome?.CloseMainWindow();
        }

        public static void OpenLink(string url, string chromeDirectory)
        {
            var psi = new ProcessStartInfo("powershell.exe", @"start chrome " + url);
            psi.WorkingDirectory = Path.GetDirectoryName(chromeDirectory);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(psi);
        }

        public static void SendButtonPressToChrome(string key)
        {
            var title = GetChromeMainWindowTitle();
            if (title != null)
            {
                var command = $"$wshell = New-Object -ComObject wscript.shell; $wshell.AppActivate('" + title + "'); Sleep 1; $wshell.SendKeys('" + key + "')";
                var psi = new ProcessStartInfo("powershell.exe", command);
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
            }
        }

        public static void CloseOtherTabs()
        {
            SendButtonPressToChrome("%+w");
        }
    }
}
