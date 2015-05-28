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
        private IDictionary<string, long> sendTimes = new Dictionary<string, long>();

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
                Console.WriteLine("onMessage: " + name);
            }
            else
            {
                sendTimes[name] = long.MaxValue;
                File.WriteAllBytes(Path.Combine(folder, name), data);
                name = null;
            }
        }

        public void Send(string name)
        {
            string path = Path.Combine(folder, name);

            long nowTime = DateTime.Now.Ticks;

            if (!sendTimes.ContainsKey(name) || nowTime > sendTimes[name])
            {
                byte[] data = TryToLoadBytes(name);

                if (data != null)
                {
                    SendFileData(name, data);
                }
                else
                {
                    Console.WriteLine("error reading: " + name);
                }
            }

            sendTimes[name] = nowTime;
        }

        private byte[] TryToLoadBytes(string name, int tryCount = 5, int tryDelay = 50)
        {
            byte[] data = null;

            do
            {
                try
                {
                    data = File.ReadAllBytes(Path.Combine(folder, name));
                }
                catch
                {
                }
                Thread.Sleep(tryDelay);
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
