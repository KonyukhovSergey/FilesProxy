using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileProxyServer.Network;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetworkUnitTests
{
    [TestClass]
    public class NetworkUnitTests
    {
        [TestMethod]
        public void TestCreate()
        {
            TestConnectionListener connectionListener = new TestConnectionListener();
            NioNetServer server = new NioNetServer(11001, connectionListener);
            server.Stop();
        }

        [TestMethod]
        public void TestConnection()
        {
            TestConnectionListener serverConnectionListener = new TestConnectionListener();
            TestConnectionListener clientConnectionListener = new TestConnectionListener();
            NioNetServer server = new NioNetServer(11001, serverConnectionListener);
            NioNetClient client = new NioNetClient("127.0.0.1", 11001, clientConnectionListener);

            client.Tick();
            server.Tick();
            client.Tick();

            Assert.AreEqual(1, serverConnectionListener.OnConnectCount);
            Assert.AreEqual(1, clientConnectionListener.OnConnectCount);

            client.Close();
            server.Stop();
        }

        private string message = "this is a test string";

        [TestMethod]
        public void TestMessages()
        {
            TestConnectionListener serverConnectionListener = new TestConnectionListener();
            TestConnectionListener clientConnectionListener = new TestConnectionListener();
            NioNetServer server = new NioNetServer(11001, serverConnectionListener);
            NioNetClient client = new NioNetClient("127.0.0.1", 11001, clientConnectionListener);

            client.Tick();
            server.Tick();
            client.Tick();

            client.Send(Encoding.Default.GetBytes(message));

            for (int i = 0; i < 5; i++)
            {
                client.Tick();
                server.Tick();
                Thread.Sleep(10);
            }

            Assert.AreEqual(1, serverConnectionListener.OnMessageCount);
            Assert.AreEqual(message, Encoding.Default.GetString(serverConnectionListener.messages.Dequeue()));

            server.Connections()[0].Send(Encoding.Default.GetBytes(message));

            for (int i = 0; i < 5; i++)
            {
                client.Tick();
                server.Tick();
                Thread.Sleep(10);
            }

            Assert.AreEqual(1, clientConnectionListener.OnMessageCount);
            Assert.AreEqual(message, Encoding.Default.GetString(clientConnectionListener.messages.Dequeue()));

            client.Close();
            server.Stop();
        }
    }

    public class TestConnectionListener : ConnectionListener
    {
        public int OnConnectCount = 0;
        public int OnDisconnectCount = 0;
        public int OnMessageCount = 0;
        public Queue<byte[]> messages = new Queue<byte[]>();

        public void OnConnect(ConnectionProvider connection)
        {
            OnConnectCount++;
        }

        public void OnDisconnect(ConnectionProvider connection)
        {
            OnDisconnectCount++;
        }

        public void OnMessage(ConnectionProvider connection, byte[] data)
        {
            messages.Enqueue(data);
            OnMessageCount++;
        }
    }
}
