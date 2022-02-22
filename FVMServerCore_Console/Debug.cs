using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVMServerCore_Console
{
    /// <summary>
    /// 调试输出类
    /// </summary>
    internal class Debug
    {
        public static Debug Instance = new Debug();

        public void AddLog(string txt, ConsoleColor color, ConsoleColor bgColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = bgColor;
            Console.WriteLine(txt);
        }
        public void Sys<T>(T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]System > {txt}", ConsoleColor.Green);
        }
        public void Usr<T>(T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Control > {txt}", ConsoleColor.Gray);
        }
        public void Log<T>(T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", ConsoleColor.White);
        }
        public void Err<T>(T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Error > {txt}", ConsoleColor.Red);
        }
        public void Print<T>(T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]Log > {txt}", ConsoleColor.Gray);
        }
        public void Client_Log<T>(string ip, T txt) where T : class
        {
            AddLog($"[{DateTime.Now.ToString("HH:mm:ss")}]{ip} > {txt}", ConsoleColor.Cyan);
        }
    }
}
