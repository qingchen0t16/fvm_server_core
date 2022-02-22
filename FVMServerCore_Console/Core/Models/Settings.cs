using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVMServerCore_Console.Models
{
    /// <summary>
    /// setting.conf 文件配置类
    /// </summary>
    public class Settings {
        public _Server server = new _Server();
        public _Play play = new _Play();
        public _DB db = new _DB();
    }

    /// <summary>
    /// 游玩设置
    /// </summary>
    public class _Play {
        public string max_players = "100",
            player_main_level = "1",
            player_main_exp_difficulty = "1",
            player_main_money_d = "100",
            player_main_money_gd = "100",
            player_main_money_g = "1000";
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public class _DB {
        public string? mysql_ip = "127.0.0.1",
            mysql_port = "3306",
            mysql_user,
            mysql_pwd,
            mysql_charset = "utf8",
            db_name = "fvm_data";
    }

    /// <summary>
    /// 服务器配置
    /// </summary>
    public class _Server{
        public string ip = "127.0.0.1",port = "78987";
    }
}
