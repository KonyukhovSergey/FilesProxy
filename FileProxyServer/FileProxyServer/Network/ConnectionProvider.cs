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

        public MessageListener MessageListener;

        public ConnectionProvider(Socket socket, MessageListener messageListener)
        {
            this.socket = socket;
            this.MessageListener = messageListener;
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(string data)
        {
            Send(Encoding.UTF8.GetBytes(data));
        }

        public void Send(byte[] data)
        {
            if (data.Length > 0)
            {
                sendQueue.Enqueue(BitConverter.GetBytes(data.Length));
                sendQueue.Enqueue(data);
            }
        }

        public void Tick()
        {
            recv(MessageListener);
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
