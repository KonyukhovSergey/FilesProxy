using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer
{
    public class BufferReceiver
    {
        private const int STATE_LENGTH = 1;
        private const int STATE_DATA = 2;

        private int position;

        private byte[] data;
        private byte[] sizebuf = new byte[4];
        private int state;

        public BufferReceiver()
        {
            init();
        }

        private void init()
        {
            position = 0;
            state = STATE_LENGTH;
        }

        public void Receive(Socket socket, ConnectionProvider connection, MessageListener message)
        {
            while (socket.Available > 0)
            {
                if (state == STATE_LENGTH)
                {
                    position += socket.Receive(sizebuf, position, 4 - position, SocketFlags.None);
                    if (position == 4)
                    {
                        state = STATE_DATA;
                        data = new byte[BitConverter.ToInt32(sizebuf, 0)];
                        position = 0;
                    }
                }
                if (state == STATE_DATA)
                {
                    position += socket.Receive(data, position, data.Length - position, SocketFlags.None);

                    if (position == data.Length)
                    {
                        message.OnMessage(connection, data);
                        init();
                    }
                }
            }
        }
    }
}
