using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer.Network
{
    public class NioNetServer
    {
        private IList<ConnectionProvider> connections = new List<ConnectionProvider>();
        private ConnectionAcceptor connectionAcceptor;
        private ConnectionListener connectionListener;

        public NioNetServer(int port, ConnectionListener connectionListener)
        {
            this.connectionListener = connectionListener;
            connectionAcceptor = new ConnectionAcceptor(port);
        }

        public void Stop()
        {
            connectionAcceptor.Close();
        }

        public void Tick()
        {
            try
            {
                Socket socketChannel = connectionAcceptor.Accept();

                if (socketChannel != null)
                {
                    ConnectionProvider client = new ConnectionProvider(socketChannel, connectionListener);
                    connectionListener.OnConnect(client);
                    connections.Add(client);
                }
            }
            catch
            {
                // TODO: it is need the decision to be or not to be
            }

            for (int i = 0; i < connections.Count; )
            {
                ConnectionProvider client = connections[i];

                try
                {
                    client.Tick();
                    i++;
                }
                catch
                {
                    connections.RemoveAt(i);
                    connectionListener.OnDisconnect(client);
                }
            }
        }

        public IList<ConnectionProvider> Connections()
        {
            return connections;
        }
    }
}
