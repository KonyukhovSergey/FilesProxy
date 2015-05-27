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

        public void tick()
        {
            switch (state)
            {
                case STATE_NONE:
                    try
                    {
                        socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                        socket.NoDelay = true;
                        socket.Blocking = false;
                        socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
                        socket.Connect(host, port);
                        state = STATE_CONNECTING;
                    }
                    catch (Exception e)
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
                            connectionProvider = new ConnectionProvider(socket);
                            state = STATE_CONNECTED;
                            connectionListener.OnConnect(connectionProvider);
                        }
                    }
                    catch (Exception e)
                    {
                        state = STATE_DISCONNECTED;
                        connectionListener.OnDisconnect(null);
                    }
                    break;

                case STATE_CONNECTED:
                    try
                    {
                        connectionProvider.Tick(connectionListener);
                    }
                    catch (Exception e)
                    {
                        state = STATE_DISCONNECTED;
                        connectionListener.OnDisconnect(connectionProvider);
                    }
                    break;

                case STATE_DISCONNECTED:
                    break;
            }
        }

        public void close()
        {
            try
            {
                socket.Close();
            }
            catch (Exception e)
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
