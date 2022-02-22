using FVMIO.Model;
using System.Data;

namespace FVMServerCore_Console.Core.Data
{
    /// <summary>
    /// 数据操控类
    /// </summary>
    internal static class FVMData
    {
        /// <summary>
        /// 初始化用户数据数据库
        /// </summary>
        public static void Init()
        {
            try
            {
                Debug.Instance.Log("连接到数据库...");
                DBHelper.Instance.Connection("SystemData");

            }
            catch (Exception ex)
            {
                Debug.Instance.Err(ex.Message);
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="ul"></param>
        /// <returns>返回UserID</returns>
        public static string Login(UserLogin ul)
        {
            string str = $"SELECT UserID FROM Data_User WHERE UserAccount = '{ul.Account}' AND UserPassword = '{ul.Password}' AND IsDelete = 'false'";
            try
            {
                if (DBHelper.Instance.Operate("SystemData").GetDataSetLen(str) == 1)
                    return "登录成功," + Convert.ToInt32(DBHelper.Instance.Operate("SystemData").ExecuteQuery(str).Tables[0].Rows[0][0]);
                else
                    return "账号或密码错误,-1";
            }
            catch (Exception ex)
            {
                Debug.Instance.Log(ex.Message);
                return "未知错误,-1";
            }
        }

        /// <summary>
        /// 获取指定用户ID的数据
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserData? GetUserData(int userID)
        {
            string str = $"SELECT UserID,UserAccount,UserName,UserSex,Level,Exp,MoneyD,MoneyGD,MoneyGB FROM Data_User WHERE UserID = '{userID}' AND IsDelete = 'false'";
            try
            {
                if (DBHelper.Instance.Operate("SystemData").GetDataSetLen(str) == 1)
                {
                    DataRow dr = DBHelper.Instance.Operate("SystemData").ExecuteQuery(str).Tables[0].Rows[0];
                    UserData userData = new UserData
                    {
                        UserID = Convert.ToInt32(dr[0]),
                        Account = dr[1].ToString(),
                        UserName = dr[2].ToString(),
                        Level = new UserLevel {
                            Level = Convert.ToInt32(dr[5]),
                            Exp = Convert.ToInt64(dr[6])
                }, 
                        Sex = dr[3].ToString(),
                        Money = new UserMoneyData
                        {
                            D = Convert.ToInt64(dr[6]),
                            GD = Convert.ToInt64(dr[7]),
                            GB = Convert.ToInt64(dr[8])
                        }
                    };
                    userData.Level.NeedExp = (long)Math.Round((userData.Level.Level * 100.99F + (userData.Level.Level - 1) * 1024) * Convert.ToDouble(FVMCore.Instance.Settings.play.player_main_exp_difficulty));
                    return userData;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Debug.Instance.Log(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string RegisterUser(RegData data)
        {
            if (DBHelper.Instance.Operate("SystemData").GetDataSetLen($"SELECT * FROM Data_User WHERE UserAccount = '{data.Account}' AND IsDelete = 'false'") != 0)
                return "账号已被注册";
            if (DBHelper.Instance.Operate("SystemData").GetDataSetLen($"SELECT * FROM Data_User WHERE UserName = '{data.NickName}' AND IsDelete = 'false'") != 0)
                return "昵称已存在";
            try
            {
                return DBHelper.Instance.Operate("SystemData").ExecuteNonQuery($"INSERT INTO Data_User(UserAccount,UserPassword,MoneyGB,MoneyGD,MoneyD,UserName,Level,Exp,UserSex,IsDelete) VALUES('{data.Account}','{data.Password}','{FVMCore.Instance.Settings.play.player_main_money_g}','{FVMCore.Instance.Settings.play.player_main_money_gd}','{FVMCore.Instance.Settings.play.player_main_money_d}','{data.NickName}','{FVMCore.Instance.Settings.play.player_main_level}','0','{data.Sex}','false')") == 1 ? "注册成功" : "注册失败";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 从数据库中获取随机名字
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNickName(string sex)
        {
            string sql = $"SELECT Name FROM Sys_Create_NickName WHERE Name NOT IN (SELECT UserName FROM Data_User WHERE IsDelete = 'false') AND Sex = '{sex}' ORDER BY RAND() LIMIT 1";
            if (DBHelper.Instance.Operate("SystemData").GetDataSetLen(sql) == 0)
                return "库中暂时没有新的昵称";
            return DBHelper.Instance.Operate("SystemData").ExecuteQuery(sql).Tables[0].Rows[0][0].ToString();
        }

        /// <summary>
        /// 发送全服邮件
        /// </summary>
        public static void SendAllUserMail(string title, string content, string reword)
        {
            // 获取全部用户数量
            for (long i = 0; i < GetAllUserNum(); i++)
                DBHelper.Instance.Operate("SystemData").ExecuteNonQuery($"INSERT INTO data_mail(data_mail.MailTitle,data_mail.MailContent,data_mail.SpanTime,data_mail.UID,data_mail.FromUID,data_mail.IsReword,data_mail.RewordItemIdList,data_mail.Checked,data_mail.CheckReword) VALUES('{title}','{content}',now(),'{i}','0','{(reword.Equals(String.Empty) ? "false" : "true")}','{reword}','false','false')");
        }

        /// <summary>
        /// 获取全部用户数量
        /// </summary>
        /// <returns></returns>
        public static long GetAllUserNum()
        {
            string sql = "SELECT COUNT(UserID) FROM Data_User";
            return (long)DBHelper.Instance.Operate("SystemData").ExecuteData(sql);
        }
    }
}
