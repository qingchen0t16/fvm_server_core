using FVMServerCore_Console;
using FVMServerCore_Console.Core;
using FVMServerCore_Console.Core.Data;
using System.Text;

// 初始化
Debug.Instance.Log("执行初始化...");
// 获取settings.conf数据
StreamReader sr = new StreamReader("Conf/settings.conf", Encoding.UTF8);
string line;
while ((line = sr.ReadLine()) != null)
    GetSettingStr(line);

if (!VerifySetting())
{
    Debug.Instance.Err($"某些不可空值未设置,请编辑settings.conf文件");
    return;
}
Debug.Instance.Log("初始化完成,服务器指令不区分大小写,随便怎么写");
FVMData.Init();
FVMCore.Instance.Init();

// 进入循环监听按键
string cmd = "";
do
{
    if (cmd.Equals(String.Empty))
        continue;
    string[] cmdList = cmd.ToLower().Split(' ');

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.BackgroundColor = ConsoleColor.Black;
    // 命令处理
    switch (cmdList[0]) {

        case "exit":    // 程序退出
            return;
        default:
            Debug.Instance.Err($"未知的指令.");
            break;
    }
} while ((cmd = Console.ReadLine()) != null);

/// <summary>
/// 获取配置文件数据
/// </summary>
void GetSettingStr(string str)
{
    if (str[0] == '#')
        return;
    string[] keyval = str.Split('=');
    if (String.Empty.Equals(keyval[1]))
    {
        Debug.Instance.Err($"{keyval[0]}值未设置,将使用默认值");
        return;
    }
    else
        Debug.Instance.Log($"{keyval[0]}:{keyval[1]}");
    switch (keyval[0])
        {
            case "server-ip":
                FVMCore.Instance.Settings.server.ip = keyval[1];
                break;
            case "server-port":
                FVMCore.Instance.Settings.server.port = keyval[1];
                break;
            case "mysql-ip":
                FVMCore.Instance.Settings.db.mysql_ip = keyval[1];
                break;
            case "mysql-port":
                FVMCore.Instance.Settings.db.mysql_port = keyval[1];
                break;
            case "mysql-user":
                FVMCore.Instance.Settings.db.mysql_user = keyval[1];
                break;
            case "mysql-pwd":
                FVMCore.Instance.Settings.db.mysql_pwd = keyval[1];
                break;
            case "mysql-charset":
                FVMCore.Instance.Settings.db.mysql_charset = keyval[1];
                break;
            case "db-name":
                FVMCore.Instance.Settings.db.db_name = keyval[1];
                break;
            case "max-players":
                FVMCore.Instance.Settings.play.max_players = keyval[1];
                break;
            case "player-main-level":
                FVMCore.Instance.Settings.play.player_main_level = keyval[1];
                break;
            case "player-main-exp-difficulty":
                FVMCore.Instance.Settings.play.player_main_exp_difficulty = keyval[1];
                break;
            case "player-main-money-d":
                FVMCore.Instance.Settings.play.player_main_money_d = keyval[1];
                break;
            case "player-main-money-gd":
                FVMCore.Instance.Settings.play.player_main_money_gd = keyval[1];
                break;
            case "player-main-money-g":
                FVMCore.Instance.Settings.play.player_main_money_g = keyval[1];
                break;
        }
}

/// <summary>
/// 判断配置文件是否符合要求
/// </summary>
bool VerifySetting() {
    if (FVMCore.Instance.Settings.db.mysql_user == String.Empty || FVMCore.Instance.Settings.db.mysql_pwd == String.Empty)
        return false;
    return true;
}