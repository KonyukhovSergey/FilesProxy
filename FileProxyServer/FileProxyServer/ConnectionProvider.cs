using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace FileProxyServer
{
    public class ConnectionProvider
    {
        private Socket socket;
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private BufferSender sender = new BufferSender();

        public ConnectionProvider(Socket socket)
        {
            this.socket = socket;
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(byte[] data)
        {
            sendQueue.Enqueue(data);
        }

        public void Tick(MessageListener connection)
        {
            recv(connection);
            send();
        }

        private void send()
        {


            while (sender.Send(socket))
            {
            };
        }

        private void recv(MessageListener connection)
        {
            throw new NotImplementedException();
        }
    }
}
