using FileProxyServer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProxyServer
{
    public class FileClient : MessageListener
    {
        private ConnectionProvider connection;
        private string folder;
        private string name;
        private IDictionary<string, DateTime> files;

        public FileClient(ConnectionProvider connection, string folder, IDictionary<string, DateTime> files)
        {
            this.connection = connection;
            connection.MessageListener = this;
            this.folder = folder;
            this.files = files;
        }

        public void OnMessage(ConnectionProvider connection, byte[] data)
        {
            if (name == null)
            {
                name = Encoding.UTF8.GetString(data);
                if (name.StartsWith("delete "))
                {
                    name = name.Substring(7);
                    File.Delete(Path.Combine(folder, name));
                    name = null;
                }
            }
            else
            {
                string path = Path.Combine(folder, name);
                File.WriteAllBytes(path, data);
                files[name] = File.GetLastWriteTime(path);
                name = null;
            }
        }

        public void Send(string name, byte[] data)
        {
            connection.Send(name);
            connection.Send(data);
        }

        public void Delete(string name)
        {
            connection.Send("delete " + name);
        }
    }
}
