using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer.Network
{
    public class ConnectionAcceptor
    {
        private TcpListener listener;

        public ConnectionAcceptor(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start(50);
        }

        public Socket Accept()
        {
            if(listener.Pending())
            {
                Socket socket = listener.AcceptSocket();
                socket.Blocking = false;
                socket.NoDelay = true;
                //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                return socket;
            }
            return null;
        }

        public void Close()
        {
            listener.Stop();
        }
    }
}
