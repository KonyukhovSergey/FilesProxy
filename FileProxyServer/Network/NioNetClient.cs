using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer.Network
{
    public class NioNetClient
    {
        private const int STATE_NONE = 0;
        private const int STATE_CONNECTING = 1;
        private const int STATE_CONNECTED = 2;
        private const int STATE_DISCONNECTED = 3;

        private ConnectionProvider connectionProvider;
        private Socket socket;
        private int state = STATE_NONE;

        private ConnectionListener connectionListener;
        private string host;
        private int port;

        public NioNetClient(string host, int port, ConnectionListener connectionListener)
        {
            this.connectionListener = connectionListener;
            this.host = host;
            this.port = port;
        }

        public bool Tick()
        {
            switch (state)
            {
                case STATE_NONE:
                    try
                    {
                        socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(host, port);
                        socket.NoDelay = true;
                        socket.Blocking = false;
                        state = STATE_CONNECTING;
                    }
                    catch
                    {
                        state = STATE_DISCONNECTED;
                        connectionListener.OnDisconnect(null);
                    }
                    break;

                case STATE_CONNECTING:
                    try
                    {
                        if (socket.Connected)
                        {
                            connectionProvider = new ConnectionProvider(socket, connectionListener);
                            state = STATE_CONNECTED;
                            connectionListener.OnConnect(connectionProvider);
                        }
                    }
                    catch
                    {
                        state = STATE_DISCONNECTED;
                        connectionListener.OnDisconnect(null);
                    }
                    break;

                case STATE_CONNECTED:
                    try
                    {
                        connectionProvider.Tick();
                    }
                    catch
                    {
                        state = STATE_DISCONNECTED;
                        connectionListener.OnDisconnect(connectionProvider);
                    }
                    break;

                case STATE_DISCONNECTED:
                    return false;
            }
            return true;
        }

        public void Close()
        {
            try
            {
                socket.Close();
            }
            catch
            {
            }
        }

        public void Send(byte[] message)
        {
            if (state == STATE_CONNECTED)
            {
                connectionProvider.Send(message);
            }
        }

    }
}
