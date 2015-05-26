using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer
{
    public interface ConnectionListener : MessageListener
    {
        void OnConnect(ConnectionProvider connection);
        void OnDisconnect(ConnectionProvider connection);
    }
}
