using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer
{
    public class BufferSender
    {
        private int position;
        private byte[] data;

        public void Init(byte[] data)
        {
            position = 0;
            this.data = new byte[data.Length + 4];
            System.Array.Copy(BitConverter.GetBytes(data.Length), this.data, 4);
            Array.Copy(data, 0, this.data, 4, data.Length);
        }

        public bool Send(Socket socket)
        {
            position += socket.Send(data, position, data.Length - position, SocketFlags.None);
            return position < data.Length;
        }
    }
}
