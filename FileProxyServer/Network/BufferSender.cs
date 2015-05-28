using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer.Network
{
    public class BufferSender
    {
        private int position;
        private byte[] data = new byte[0];

        public void Init(byte[] data)
        {
            position = 0;
            this.data = data;
        }

        public bool IsSent
        {
            get
            {
                return position == data.Length;
            }
        }

        public void Send(Socket socket)
        {
            position += socket.Send(data, position, data.Length - position, SocketFlags.None);
        }
    }
}
