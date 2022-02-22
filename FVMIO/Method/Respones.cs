using FVMIO.Enum;
using FVMIO.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FVMIO.Method
{
    /// <summary>
    /// 发送数据类
    /// </summary>
    public class Respones
    {
        public Dictionary<long, SourcePackage> Replys = new Dictionary<long, SourcePackage>(); // 请求返回数据
        private byte[] sourceID = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; // 数据发送ID
        private long SourceID
        {
            get
            {
                return BitConverter.ToInt64(sourceID);   // byte转换为int后输出
            }
            set => sourceID = BitConverter.GetBytes(value);   // int转回byte 塞给sourceID
        }

        /// <summary>
        /// 发送数据到指定Socket
        /// </summary>
        /// <param name="requestSocket"></param>
        /// <param name="sendType"></param>
        /// <param name="sendObj"></param>
        /// <param name="header"></param>
        /// <param name="sourceID"></param>
        /// <returns>返回SourceID,-1:发送失败</returns>
        public long Send<T>(Socket requestSocket, SendType sendType, T sendObj, Action<SourcePackage> endReceive, string header = "Null", long sendSourceID = -1)
        {
            if (!requestSocket.Connected)
                return -1;
            byte[] buff = Data.ObjToXml<T>(sendObj);
            /**
             * 包结构 byte[1024]
             *  0-7      SourceID
             *  8        包类型
             *  9-38     包头
             *  39-42    包总数
             *  43-46    包索引
             *  47-50    数据报长
             *  51-1024  包数据
             */
            List<byte> sendBuff = new List<byte>();
            sendBuff.AddRange(sendSourceID == -1 ? sourceID : BitConverter.GetBytes(sendSourceID));     // 把SourceID塞进去
            sendBuff.Add((byte)sendType);    // sendType
            byte[] headByte = new byte[30],
                   headByteOld = Encoding.UTF8.GetBytes(header);
            Array.Copy(headByteOld, 0, headByte, 0, headByteOld.Length);
            // head长度大于31 不给予发送
            if (headByteOld.Length > 30)
                return -1;
            sendBuff.AddRange(headByte);    // 加入包Head
            int sendCount = buff.Length % 973 == 0 ? (buff.Length / 973) : (buff.Length / 973 + 1); // 总发包数
            sendBuff.AddRange(BitConverter.GetBytes(sendCount)); // 塞进去包总数
            for (int i = 0; i < sendCount; i++)
            {
                sendBuff.RemoveRange(43, sendBuff.Count - 43);
                byte[] temp;
                int len = 973;
                if (buff.Length < (i + 1) * 973) // 当前length不够240长度
                    len = (buff.Length - i * 973);    // 长度则变成剩余长度
                temp = buff.Skip(i * 973).Take(len).ToArray();    // 获取当前字段byte内容
                sendBuff.AddRange(BitConverter.GetBytes(i));    // 塞入第几个包
                sendBuff.AddRange(BitConverter.GetBytes(len));    // 塞入报长
                sendBuff.AddRange(temp);    // 塞入数据
                requestSocket.Send(sendBuff.ToArray());
            }
            Console.WriteLine(BitConverter.ToString(sendBuff.ToArray()));
            Replys.Add(sendSourceID == -1 ? SourceID : sendSourceID, new SourcePackage(requestSocket, endReceive));
            return sendSourceID == -1 ? SourceID++ : sendSourceID;  // 返回SourceID
        }
        public long Send<T>(Socket requestSocket, SendType sendType, T sendObj, string header = "Null", long sendSourceID = -1)
        {
            if (!requestSocket.Connected)
                return -1;
            byte[] buff = Data.ObjToXml(sendObj);
            /**
             * 包结构 byte[1024]
             *  0-7      SourceID
             *  8        包类型
             *  9-38     包头
             *  39-42    包总数
             *  43-46    包索引
             *  47-50    数据报长
             *  51-1024  包数据
             */
            List<byte> sendBuff = new List<byte>();
            sendBuff.AddRange(sendSourceID == -1 ? sourceID : BitConverter.GetBytes(sendSourceID));     // 把SourceID塞进去
            sendBuff.Add((byte)sendType);    // sendType
            byte[] headByte = new byte[30],
                   headByteOld = Encoding.UTF8.GetBytes(header);
            Array.Copy(headByteOld, 0, headByte, 0, headByteOld.Length);
            // head长度大于31 不给予发送
            if (headByteOld.Length > 30)
                return -1;
            sendBuff.AddRange(headByte);    // 加入包Head
            int sendCount = buff.Length % 973 == 0 ? (buff.Length / 973) : (buff.Length / 973 + 1); // 总发包数
            sendBuff.AddRange(BitConverter.GetBytes(sendCount)); // 塞进去包总数
            for (int i = 0; i < sendCount; i++)
            {
                sendBuff.RemoveRange(43, sendBuff.Count - 43);
                byte[] temp;
                int len = 973;
                if (buff.Length < (i + 1) * 973) // 当前length不够240长度
                    len = (buff.Length - i * 973);    // 长度则变成剩余长度
                temp = buff.Skip(i * 973).Take(len).ToArray();    // 获取当前字段byte内容
                sendBuff.AddRange(BitConverter.GetBytes(i));    // 塞入第几个包
                sendBuff.AddRange(BitConverter.GetBytes(len));    // 塞入报长
                sendBuff.AddRange(temp);    // 塞入数据
                requestSocket.Send(sendBuff.ToArray());
            }
            Console.WriteLine(BitConverter.ToString(sendBuff.ToArray()));
            return sendSourceID == -1 ? SourceID++ : sendSourceID;  // 返回SourceID
        }

        /// <summary>
        /// 批量发送数据到Socket
        /// </summary>
        /// <param name="requestSockets"></param>
        /// <param name="sendType"></param>
        /// <param name="sendObj"></param>
        /// <param name="header"></param>
        /// <param name="sourceID"></param>
        /// <returns>返回发送成功个数</returns>
        public int BatchSend<T>(Socket[] requestSockets, SendType sendType, T sendObj, Action<SourcePackage> endReceive, string header = "Reply", long sendSourceID = -1)
        {
            int sendCount = 0;
            foreach (Socket socket in requestSockets)
                sendCount += Send<T>(socket, sendType, sendObj, endReceive, header, sendSourceID) == -1 ? 0 : 1;
            return sendCount;
        }
        public int BatchSend<T>(Socket[] requestSockets, SendType sendType, T sendObj, string header = "Reply", long sourceID = -1, long sendSourceID = -1)
        {

            int sendCount = 0;
            foreach (Socket socket in requestSockets)
                sendCount += Send<T>(socket, sendType, sendObj, header, sendSourceID) == -1 ? 0 : 1;
            return sendCount;
        }
    }
}
