using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer.Network
{
    public interface ConnectionListener : MessageListener
    {
        void OnConnect(ConnectionProvider connection);
        void OnDisconnect(ConnectionProvider connection);
    }
}
