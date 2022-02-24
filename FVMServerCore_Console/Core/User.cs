using FVMIO_From_Standard2_0.API;
using FVMIO_From_Standard2_0.Enum;
using FVMIO_From_Standard2_0.Model;
using FVMIO_From_Standard2_0.Package;
using FVMServerCore_Console.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FVMServerCore_Console.Core
{
    public class User : IRequest
    {
        public bool IsLogin = false;    // 是否登录
        public string? Account = null;  // 用户账号

        public User(Socket request)
        {
            Request = request;
            Request.BeginReceive(buff, 0, 1024, SocketFlags.None, new AsyncCallback(DataEnter), Request);  // 处理请求
        }

        public override void DataEnterEx(Exception ex) => Debug.Instance.Client_Log($"{IP}:{Port}", $"引发异常 > {ex.Message}");

        public override void PagePushFailed(byte[] buff) => Debug.Instance.Client_Log($"{IP}:{Port}", $"数据包解析出现问题: " + $"数据长度:{buff.Length}");

        public override void Request_Close(string exMessage)
        {
            Debug.Instance.Client_Log($"{(IPEndPoint)Request.RemoteEndPoint}", $"引发异常导致客户退出 > {exMessage}");
            FVMCore.Instance.Clients.Remove(Request);   // 从用户池中删除这个用户
        }

        public override void EndReceive(SourcePackage sp)
        {
            switch (sp.SendType)
            {
                case SendType.Text:
                    if (sp.Header == "GetRandomName")   // 随机名字
                    {
                        Debug.Instance.Client_Log($"{Account ?? IP + ":" + Port}", $"({sp.SourceID}){sp.Header} > 获取随机用户名");
                        Respones.Send(sp.RequestSocket, SendType.Reply, FVMData.GetRandomNickName(sp.GetSource<string>()), "Null", sp.SourceID);
                    }
                    if (sp.Header == "GetUserData") // 获取User基础数据
                    {
                        Debug.Instance.Client_Log($"{Account ?? IP + ":" + Port}", $"[SourceID:{sp.SourceID}]{sp.Header} > 获取用户数据");
                        Respones.Send<UserData>(sp.RequestSocket, SendType.Reply, FVMData.GetUserData(Convert.ToInt32(sp.GetSource<string>())) is null ? new UserData { UserID = -1 } : FVMData.GetUserData(Convert.ToInt32(sp.GetSource<string>())), "Null", sp.SourceID);
                    }
                    break;
                case SendType.Object:
                    if (sp.Header == "Login")   // 登录
                    {
                        UserLogin ul = sp.GetSource<UserLogin>();
                        Debug.Instance.Client_Log($"{Account ?? IP + ":" + Port}", $"[SourceID:{sp.SourceID}]{sp.Header} > 尝试登陆");
                        string temp = FVMData.Login(ul);
                        Respones.Send(sp.RequestSocket, SendType.Reply, temp, "Null", sp.SourceID);
                        if(temp.Split(',')[0].Equals("登录成功"))
                            Account = ul.Account;
                    }
                    if (sp.Header == "Register")    // 注册
                    {
                        RegData data = sp.GetSource<RegData>();
                        Debug.Instance.Client_Log($"{Account ?? IP + ":" + Port}", $"[SourceID:{sp.SourceID}]{sp.Header} > 尝试注册");
                        Respones.Send(sp.RequestSocket, SendType.Reply, FVMData.RegisterUser(data), "Null", sp.SourceID);
                    }
                    if (sp.Header == "Message")
                    {
                        SendMessage data = sp.GetSource<SendMessage>();
                        Debug.Instance.Client_Log($"{Account ?? IP + ":" + Port}", $"[SourceID:{sp.SourceID}]{sp.Header}{data.SendType} > {data.Message}");
                        Respones.BatchSend(FVMCore.Instance.Clients.Keys.ToArray(), SendType.Object, data, "Message", sp.SourceID);
                    }
                    break;
            }
        }
    }
}
