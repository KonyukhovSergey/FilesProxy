using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using FileProxyServer.Network;
using System.IO;
using System.Diagnostics;

namespace FileProxyServer
{
    public partial class ServerForm : Form, ConnectionListener
    {
        private NioNetServer server;
        private Tools.IniFile ini = new Tools.IniFile();

        private IList<FileClient> clients = new List<FileClient>();

        private FileSystemWatcher watcher;
        private FileSystemEventHandler watcherHandler;

        public ServerForm()
        {
            InitializeComponent();
            SettingsLoad();
        }

        protected override void OnClosed(EventArgs e)
        {
            SettingsSave();
            base.OnClosed(e);
        }

        private void SettingsSave()
        {
            ini.Write("folder", textBoxFolder.Text);
            ini.Write("port", numericUpDownPort.Value.ToString());
        }

        private void SettingsLoad()
        {
            if (ini.KeyExists("folder")) { textBoxFolder.Text = ini.Read("folder"); }
            if (ini.KeyExists("port")) { numericUpDownPort.Value = decimal.Parse(ini.Read("port")); }
        }

        private void NetworkTimer_Tick(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.Tick();
            }
        }

        public void OnConnect(ConnectionProvider connection)
        {
            clients.Add(new FileClient(connection, textBoxFolder.Text));
        }

        public void OnDisconnect(ConnectionProvider connection)
        {
            clients.Remove((FileClient)connection.MessageListener);
        }

        public void OnMessage(ConnectionProvider connection, byte[] data)
        {
            throw new NotImplementedException();
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            ServerStartStop();
        }

        private void ServerStartStop()
        {
            if (server == null)
            {
                server = new NioNetServer((int)numericUpDownPort.Value, this);
                groupBoxSettings.Enabled = false;
                labelStatus.Text = "Working";
                buttonStartStop.Text = "Stop";

                watcher = new FileSystemWatcher(textBoxFolder.Text + "\\");
                watcher.InternalBufferSize = 32768;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcherHandler = new FileSystemEventHandler(watcher_Changed);
                watcher.Changed += watcherHandler;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                server.Stop();
                server = null;
                groupBoxSettings.Enabled = true;
                labelStatus.Text = "Stopped";
                buttonStartStop.Text = "Start";

                watcher.EnableRaisingEvents = false;
                watcher.Changed -= watcherHandler;
                watcher.Dispose();
            }
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //Debug.WriteLine(e.ChangeType.ToString() + " " + e.Name + " ");
            foreach (var client in clients)
            {
                client.Send(e.Name);
            }
        }

        private void buttonRootFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowseDialog = new FolderBrowserDialog();
            folderBrowseDialog.SelectedPath = textBoxFolder.Text;
            if (folderBrowseDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowseDialog.SelectedPath.ToLower();
            }
        }
    }
}
