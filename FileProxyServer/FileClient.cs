using FileProxyServer.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IDictionary<string, DateTime> files = new Dictionary<string, DateTime>();

        public FileClient(ConnectionProvider connection, string folder)
        {
            this.connection = connection;
            connection.MessageListener = this;
            this.folder = folder;
        }

        public void OnMessage(ConnectionProvider connection, byte[] data)
        {
            if (name == null)
            {
                name = Encoding.UTF8.GetString(data);
            }
            else
            {
                string path = Path.Combine(folder, name);
                File.WriteAllBytes(path, data);
                files[name] = File.GetLastWriteTime(path);
                name = null;
            }
        }

        public void Send(string name)
        {
            string path = Path.Combine(folder, name);
            DateTime lastWriteTime = File.GetLastWriteTime(path);

            if (files[name] != lastWriteTime)
            {
                files[name] = lastWriteTime;
                SendFileData(name, File.ReadAllBytes(path));
            }
        }

        private void SendFileData(string name, byte[] data)
        {
            Debug.WriteLine("sending: " + name);
            connection.Send(name);
            connection.Send(data);
        }

        public void OnDisconnect(ConnectionProvider connection)
        {
            throw new NotImplementedException();
        }
    }
}
