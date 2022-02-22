using FVMIO.Enum;
using FVMIO.Method;
using FVMIO.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FVMIO.API
{
    abstract public class IRequest
    {
        // 属性
        public Socket Request; // 请求的Socket 可能是客户端 也可能是服务端
        public string IP { get => ((IPEndPoint)Request.RemoteEndPoint).Address.ToString(); }    // 请求Socket的IP
        public string Port { get => ((IPEndPoint)Request.RemoteEndPoint).Port.ToString(); }     // 请求Socket的端口
        public byte[] buff = new byte[1024];    // 请求单次传入数据
        private List<byte[]> buffCookie = new List<byte[]>();   // 防止数据连接 粘包
        protected Dictionary<long, SourcePackage> pages = new Dictionary<long, SourcePackage>();   // 接收的包体(根据SourceID)
        public Respones Respones = new Respones();

        /// <summary>
        /// 数据进入
        /// </summary>
        /// <param name="ar"></param>
        public void DataEnter(IAsyncResult ar)
        {
            if (!Request.Connected)
            {
                Request_Close("未知原因断开的连接");
                return;
            }
            try
            {
                var pageLen = Request.EndReceive(ar);    // 获取数据长度
                if (pageLen == 0)    // 如果数据长度为0 掉线
                {
                    Request_Close("网络原因(掉线)断开的连接");
                    return;
                }
                long sourceID = SourcePackage.GetSourceID(buff);    // 取出sourceID
                buffCookie.Add(buff.Take(pageLen).ToArray());
                int index = buffCookie.Count - 1;
                if (SourcePackage.GetSendType(buff) != SendType.Reply)
                {
                    // 没接受过这个SourceID的包
                    if (!pages.ContainsKey(sourceID))
                        pages.Add(sourceID, new SourcePackage(Request, EndReceive));

                    if (!pages[sourceID].Push(buffCookie[index]))  // 错误的数据
                        PagePushFailed(buffCookie[index]);
                }
                else if (Respones.Replys.ContainsKey(sourceID))
                {
                    if (!Respones.Replys[sourceID].Push(buffCookie[index]))  // 错误的数据
                        PagePushFailed(buffCookie[index]);
                }
                buffCookie.RemoveAt(index);

                buff = new byte[1024];
                Request.BeginReceive(buff, 0, 1024, SocketFlags.None, new AsyncCallback(DataEnter), Request); // 监听并处理新的Request请求
            }
            catch (Exception ex)
            {
                // 抛出异常
                DataEnterEx(ex);
            }
        }

        /// <summary>
        /// DataEnter 异常处理
        /// </summary>
        /// <param name="ex"></param>
        abstract public void DataEnterEx(Exception ex);

        /// <summary>
        /// 无法解析数据
        /// </summary>
        /// <param name="buff"></param>
        abstract public void PagePushFailed(byte[] buff);

        /// <summary>
        /// 请求的Socket关闭
        /// </summary>
        /// <param name="exMessage">异常信息</param>
        abstract public void Request_Close(string exMessage);

        /// <summary>
        /// 包体接收完毕处理
        /// </summary>
        /// <param name="sp"></param>
        abstract public void EndReceive(SourcePackage sp);
    }
}
