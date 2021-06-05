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
        public log4net.ILog log;

        public FrmAICListener()
        {
            log4net.Config.BasicConfigurator.Configure();

            log = log4net.LogManager.GetLogger(typeof(Program));

            InitializeComponent();

            log.Info("form constructor");
        }

        private void txtKetNoi_Click(object sender, EventArgs e)
        {
            try
            {

                _signalR = WebApp.Start<Startup>(txtServerAIC.Text);

                btnKetNoi.Enabled = false;
                txtServerAIC.Enabled = false;
                btnHuy.Enabled = true;

                writeToLog($"{DateTime.Now} - Server started at: {txtServerAIC.Text}");
                log.Info($"txtKetNoi_Click -> Server started at: {txtServerAIC.Text}");
            }
            catch (Exception ex)
            {
                log.Error($"txtKetNoi_Click -> {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void writeToLog(string log)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    string[] row = { log };
                    var listViewItem = new ListViewItem(row);
                    lvLichSu.Items.Add(listViewItem);
                }));
            }
            else
            {
                string[] row = { log };
                var listViewItem = new ListViewItem(row);
                lvLichSu.Items.Add(listViewItem);
            }
        }
    }
}
