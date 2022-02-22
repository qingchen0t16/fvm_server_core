using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace FVMServerCore
{
    internal class Debug
    {
        public static Debug Instance = new Debug();

        public void AddLog(string txt, Color color)
        {
            Run run = new Run()
            {
                Text = txt,
                Foreground = new SolidColorBrush(color),
                FontSize = 16,
            };
            Paragraph paragraph = new Paragraph
            {
                LineHeight = 1,
            };
            paragraph.Inlines.Add(run);
            MainWindow.Ctl_Console_Out.Document.Blocks.Add(paragraph);
            MainWindow.Ctl_Console_Out.UpdateLayout();
            MainWindow.Ctl_Console_Out.CaretPosition = MainWindow.Ctl_Console_Out.Document.ContentEnd;
            MainWindow.Ctl_Console_Out.ScrollToEnd();
        }
        public void Sys(string txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]System > {txt}", Color.FromArgb(255, 47, 79, 79));
        }
        public void Usr(string txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Control > {txt}", Color.FromArgb(255, 60, 179, 113));
        }
        public void Log(string txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", Color.FromArgb(255, 105, 105, 105));
        }
        public void Err(string txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Error > {txt}", Color.FromArgb(255, 255, 0, 0));
        }
        public void Print(string txt, Color color)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", color);
        }
        public void Sys(int txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]System > {txt}", Color.FromArgb(255, 47, 79, 79));
        }
        public void Usr(int txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Control > {txt}", Color.FromArgb(255, 60, 179, 113));
        }
        public void Log(int txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", Color.FromArgb(255, 105, 105, 105));
        }
        public void Err(int txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Error > {txt}", Color.FromArgb(255, 255, 0, 0));
        }
        public void Print(int txt, Color color)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", color);
        }
        public void Client_Log(string ip, string txt)
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]{ip} > {txt}", Color.FromArgb(255, 105, 105, 105));
        }
    }
}
