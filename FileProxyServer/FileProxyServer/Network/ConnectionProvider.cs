using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace FileProxyServer.Network
{
    public class ConnectionProvider
    {
        private Socket socket;
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private BufferSender sender = new BufferSender();
        private BufferReceiver receiver = new BufferReceiver();

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
            if (data.Length > 0)
            {
                sendQueue.Enqueue(BitConverter.GetBytes(data.Length));
                sendQueue.Enqueue(data);
            }
        }

        public void Tick(MessageListener connection)
        {
            recv(connection);
            send();
        }

        private void send()
        {
            if (!sender.IsSent)
            {
                sender.Send(socket);
            }

            while (sender.IsSent && sendQueue.Count > 0)
            {
                sender.Init(sendQueue.Dequeue());
                sender.Send(socket);
            }
        }

        private void recv(MessageListener messageListener)
        {
            receiver.Receive(socket, this, messageListener);
        }
    }
}
