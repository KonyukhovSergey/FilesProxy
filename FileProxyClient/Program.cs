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

        private FileSystemWatcher fsw;

        public Client(string host,int port,string folder)
        {
            client = new NioNetClient(host, port, this);
            this.folder = folder;
            fsw = new FileSystemWatcher(folder + "\\");
            fsw.InternalBufferSize = 32768;
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Changed += fsw_Changed;
        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            fileClient.Send(e.Name);
        }

        public void OnConnect(ConnectionProvider connection)
        {
            fileClient = new FileClient(connection, folder);
            fsw.EnableRaisingEvents = true;

        }

        public void OnDisconnect(ConnectionProvider connection)
        {
            fsw.EnableRaisingEvents = false;
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
