using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AICListener
{
    public partial class FrmAICListener : Form
    {
        private IDisposable _signalR;
        private BindingList<ClientItem> _clients = new BindingList<ClientItem>();

        public log4net.ILog _log;

        public FrmAICListener()
        {
            log4net.Config.BasicConfigurator.Configure();
            _log = log4net.LogManager.GetLogger(typeof(Program));

            InitializeComponent();

            _log.Info("form constructor");
        }

        private void writeToLog(string log)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    var logDisplay = $"{DateTime.Now} - {log}";
                    
                    string[] row = { logDisplay };
                    var listViewItem = new ListViewItem(row);
                    lvLichSu.Items.Add(listViewItem);
                }));
            }
            else
            {
                var logDisplay = $"{DateTime.Now} - {log}";

                string[] row = { logDisplay };
                var listViewItem = new ListViewItem(row);
                lvLichSu.Items.Add(listViewItem);
            }
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                _signalR = WebApp.Start<Startup>(txtServerAIC.Text);

                btnKetNoi.Enabled = false;
                txtServerAIC.Enabled = false;
                btnHuy.Enabled = true;

                _log.Info($"Server started at: {txtServerAIC.Text}");
                writeToLog($"Server started at: {txtServerAIC.Text}");
            }
            catch (Exception ex)
            {
                _log.Error($"txtKetNoi_Click -> {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            _clients.Clear();

            ServerHub.ClearState();

            if(_signalR != null)
            {
                _signalR.Dispose();
                _signalR = null;

                btnHuy.Enabled = false;
                btnKetNoi.Enabled = true;
                txtServerAIC.Enabled = true;

                _log.Info("Server stopped");
                writeToLog("Server stopped");
            }
        }
    }
}
