using FileProxyServer.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

            if (!files.ContainsKey(name) || files[name] != lastWriteTime)
            {
                byte[] data = TryToLoadBytes(name);

                if (data != null)
                {
                    files[name] = lastWriteTime;
                    SendFileData(name, data);
                }
                else
                {
                    Debug.WriteLine("error reading: " + name);
                }
            }
        }

        private byte[] TryToLoadBytes(string name, int tryCount = 5, int tryDelay = 50)
        {
            byte[] data = null;

            do
            {
                try
                {
                    Thread.Sleep(tryDelay);
                    data = File.ReadAllBytes(Path.Combine(folder, name));
                }
                catch
                {
                }
                tryCount--;
            }
            while (data == null && tryCount >= 0);

            return data;
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
