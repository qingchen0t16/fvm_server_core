using FVMServerCore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FVMServerCore
{
    // 指令类
    class Cmd
    {
        public enum CmdLevel
        {
            Base,   // 普通
            Advanced,   // 高级指令
            Special, // 特殊
        }
        public string Title, Des;
        public Action<RichTextBox,string> Fun;  // 参数1 输入的指令
        public Color color;
        public Cmd(string title, string des, Action<RichTextBox, string> fun, CmdLevel level)
        {
            Title = title;
            Des = des;
            Fun = fun;
            switch (level)
            {
                case CmdLevel.Base:
                    color = Colors.Gray;
                    break;
                case CmdLevel.Advanced:
                    color = Colors.Cyan;
                    break;
                case CmdLevel.Special:
                    color = Colors.Brown;
                    break;
            }
        }
    }
    internal class Command
    {
        public static Command Instance = new Command();
        public Dictionary<string, Cmd> commandList = new Dictionary<string, Cmd>();
        // 初始化
        public void Init(ref RichTextBox txtBox)
        {
            Debug.Instance.Log( "初始化指令部分...");
            commandList.Add("CommandList", new Cmd("获取命令列表", "查看全部命令与命令信息", CommandList, Cmd.CmdLevel.Base));
            commandList.Add("Open", new Cmd("开启服务端", "开启服务端\n\t\t\t用法:Open <IP(默认127.0.0.1)> <Port(默认25565)>", Open, Cmd.CmdLevel.Special));
            commandList.Add("Close", new Cmd("关闭服务端", "停止服务端", Close, Cmd.CmdLevel.Special));
            commandList.Add("/s", new Cmd("/s <message>", "发送数据到服务端", SendMessage, Cmd.CmdLevel.Special));
            Debug.Instance.Log($"共加载{commandList.Count}个指令，输入 CommandList 查看全部命令与命令信息");
        }
        public void SendMessage(RichTextBox txtBox, string cmd) {
            if (!FVMCore.Instance.Open)
                Debug.Instance.Log("服务器未开启");
            else {
                string[] cmdList = cmd.Split("/s ");
                if (cmdList.Length != 2)
                    Debug.Instance.Log("指令错误");
                else
                {
                    FVMCore.Instance.SendSysMessageToClient(cmdList[1]);
                    Debug.Instance.Log("发送成功");
                }
            }
        }

        // 指令进入
        public void CommandEnter(RichTextBox txtBox,string cmd)
        {
            string[] cmdList = cmd.Split(' ');
            if (commandList.ContainsKey(cmdList[0])) commandList[cmdList[0]].Fun(txtBox,cmd);
            else Debug.Instance.Err( "指令不存在。");
        }
        // 指令实现
        public void CommandList(RichTextBox txtBox,string cmd)
        {
            foreach (KeyValuePair<string, Cmd> item in commandList)
                Debug.Instance.Print( $"[{item.Key}]{item.Value.Title} : {item.Value.Des}",item.Value.color);
        }
        // 启动服务端
        public void Open(RichTextBox txtBox, string cmd)
        {
            string[] cmdList = cmd.Split(" ");
            if (cmdList.Length == 1) {
                FVMCore.Instance.Init();
            }
            else if (cmdList.Length == 2)
            {
                FVMCore.Instance.Init(cmdList[1]);
            }
            else if (cmdList.Length == 3)
            {
                try
                {
                    FVMCore.Instance.Init(cmdList[1], Convert.ToInt32(cmdList[2]));
                }
                catch (Exception ex)
                {
                    Debug.Instance.Log(ex.Message);
                }
            }
        }
        // 关闭服务端
        public void Close(RichTextBox txtBox, string cmd)
        {

        }
        // 增加指令
        public void AddCommand(RichTextBox txtBox, string command, string title, string des, Action<RichTextBox, string> action, Cmd.CmdLevel level, string mod = "System")
        {
            if (commandList.ContainsKey(command))
                Debug.Instance.Log( $"{mod} 加入了一个冲突的指令 : {command}，无法正常加入");
            else
            {
                commandList.Add(command, new Cmd(title, des, action, level));
                Debug.Instance.Log( $"{mod} 新加入一个命令 : {command}");
            }
        }

        // 寻找最贴切的指令
        public string GetCommand(string cmd) {
            if (cmd.Equals(String.Empty))
                return "";
            foreach (KeyValuePair<string, Cmd> item in commandList)
            {
                if (item.Key.IndexOf(cmd) != -1)
                    return item.Key;
            }
            return "";
        }
    }
}
