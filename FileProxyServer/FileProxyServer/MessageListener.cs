using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer
{
    public interface MessageListener
    {
        void OnMessage(ConnectionProvider connection, byte[] data);
    }
}
