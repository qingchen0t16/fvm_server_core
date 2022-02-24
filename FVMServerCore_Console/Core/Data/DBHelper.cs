using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVMServerCore_Console.Core.Data
{
    public class DBHelper
    {
        // 单例
        public static readonly DBHelper Instance = new DBHelper();
        // 数据库池
        private Dictionary<string, MySqlConnection> connList = new Dictionary<string, MySqlConnection>();

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="exThrow"></param>
        /// <returns></returns>
        public bool Connection(string dbName,  bool exThrow = false)
        {
            try
            {
                connList.Add(dbName, new MySqlConnection($"server={FVMCore.Instance.Settings.db.mysql_ip};port={FVMCore.Instance.Settings.db.mysql_port};user={FVMCore.Instance.Settings.db.mysql_user};password={FVMCore.Instance.Settings.db.mysql_pwd};database={FVMCore.Instance.Settings.db.db_name};Charset={FVMCore.Instance.Settings.db.mysql_charset};"));
                Debug.Instance.Log("连接成功");
            }
            catch (Exception ex)
            {
                if (exThrow)
                    throw;
                Debug.Instance.Err(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 操作数据库
        /// 如果数据库名称没用过 会抛出异常
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public DBOperate Operate(string dbName)
        {
            try
            {
                connList[dbName].Open();
                return new DBOperate(connList[dbName]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 数据库是否已连接
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public bool Exist(string dbName)
        {
            return connList.ContainsKey(dbName);
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="exThrow"></param>
        /// <returns></returns>
        public bool CloseConnection(string dbName, bool exThrow = false)
        {
            if (connList.ContainsKey(dbName))
                return false;
            try
            {
                connList[dbName].Close();
                connList.Remove(dbName);
            }
            catch (Exception)
            {
                if (exThrow)
                    throw;
                return false;
            }
            return true;
        }
    }

    public class DBOperate
    {
        private MySqlConnection conn;

        public DBOperate(MySqlConnection conn) => this.conn = conn;

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool IsTable(string tableName)
        {
            object temp = ExecuteData($"SELECT COUNT(*) FROM sqlite_master where type ='table' and name = '{tableName}'");
            return (temp != null ? Convert.ToInt32(temp) : 0) != 0;
        }

        /// <summary>
        /// 获取单行单列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteData(string sql)
        {
            DataSet ds = ExecuteQuery(sql);
            conn.Close();
            if (ds.Tables[0].Rows.Count != 0)
                return ds.Tables[0].Rows[0][0];
            else
                return null;
        }

        /// <summary>
        /// 执行数据库查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string sql)
        {
            DataSet ds = new DataSet();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            da.Fill(ds);
            conn.Close();
            return ds;
        }

        /// <summary>
        /// 执行增 删 改语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result;
        }

        /// <summary>
        /// 返回dataSet的数据长度(只能返回表0的)
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public int GetDataSetLen(DataSet ds)
        {
            return ds.Tables[0].Rows.Count;
        }
        public int GetDataSetLen(string sql)
        {
            return ExecuteQuery(sql).Tables[0].Rows.Count;
        }
    }
}
