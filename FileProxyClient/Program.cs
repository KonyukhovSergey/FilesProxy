using FileProxyServer;
using FileProxyServer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileProxyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client(args[0], int.Parse(args[1]), args[2]);

            while (client.Tick())
            {
                Thread.Sleep(100);
            }
        }
    }

    class Client : ConnectionListener
    {
        private NioNetClient client;
        private string folder;
        private FileClient fileClient;

        private FileSystemWatcher fileSystemWatcher;
        private IDictionary<string, DateTime> filesWriteTime = new Dictionary<string, DateTime>();

        public Client(string host, int port, string folder)
        {
            client = new NioNetClient(host, port, this);
            this.folder = folder;
            fileSystemWatcher = new FileSystemWatcher(folder + "\\");
            fileSystemWatcher.InternalBufferSize = 32768;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
        }

        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            DateTime writeTime = File.GetLastWriteTime(e.FullPath);

            if (!filesWriteTime.ContainsKey(e.Name) || !writeTime.Equals(filesWriteTime[e.Name]))
            {
                filesWriteTime[e.Name] = writeTime;
                fileClient.Send(e.Name);
            }
        }

        public void OnConnect(ConnectionProvider connection)
        {
            fileClient = new FileClient(connection, folder);
            fileSystemWatcher.EnableRaisingEvents = true;

        }

        public void OnDisconnect(ConnectionProvider connection)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            client.Close();
        }

        public void OnMessage(ConnectionProvider connection, byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool Tick()
        {
            return client.Tick();
        }
    }
}
