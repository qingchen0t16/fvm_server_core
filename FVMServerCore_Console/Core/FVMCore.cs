using FVMIO_From_Standard2_0.Enum;
using FVMIO_From_Standard2_0.Model;
using FVMServerCore_Console.Models;
using System.Net;
using System.Net.Sockets;

namespace FVMServerCore_Console.Core
{
    /// <summary>
    /// 编写: qc
    /// 版本: 0.0.0
    /// 最后更新时间: 22.01.19
    /// </summary>
    public class FVMCore
    {
        public static FVMCore Instance = new FVMCore();
        // 属性
        public Settings Settings = new Settings();
        public bool Open = false; // 服务器是否开启
        public Socket server;   // Socket
        public Dictionary<Socket, User> Clients = new Dictionary<Socket, User>();
        public byte[] sourceID = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; // 数据发送ID
        private long SourceID
        {
            get
            {
                return BitConverter.ToInt64(sourceID, 0);   // byte转换为int后输出
            }
            set => sourceID = BitConverter.GetBytes(value);   // int转回byte 塞给sourceID
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Open = true;
            Debug.Instance.Log("尝试开启服务器..");
            try
            {
                Debug.Instance.Log("创建Socket...");
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Debug.Instance.Log($"监听IP {Settings.server.ip}:{Settings.server.port}");
                server.Bind(new IPEndPoint(IPAddress.Parse(Settings.server.ip), Convert.ToInt32(Settings.server.port)));
                Debug.Instance.Log($"最大同时玩家数:{Settings.play.max_players}");
                server.Listen(Convert.ToInt32(Settings.play.max_players));
                Debug.Instance.Log("开始监听...");
                server.BeginAccept(new AsyncCallback(ClientAccepted), server);
            }
            catch (Exception ex)
            {
                Debug.Instance.Err(ex.Message);
            }
        }

        /// <summary>
        /// 用户请求
        /// </summary>
        /// <param name="ar"></param>
        private void ClientAccepted(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket,    // 拿回服务端socket
                   client = socket.EndAccept(ar);       // 拿回客户端socket
            if (!Clients.ContainsKey(client))
            {
                Clients.Add(client, new User(client));
                Debug.Instance.Log($"用户{client.RemoteEndPoint}连入服务器");
            }
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);  // 等待下一个Client
        }

        /// <summary>
        /// 发送系统信息
        /// </summary>
        /// <param name="content"></param>
        public void SendSysMessageToClient(string content)
        {
            foreach (User client in Clients.Values)
            {
                if (client.IsLogin)
                    client.Respones.Send(client.Request, SendType.Object, new SendMessage
                    {
                        Id = -1,
                        Message = content,
                        SendType = "系统"
                    }, "Message");
            }
        }
    }
}
