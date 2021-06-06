
using AICListener.Code;
using AICListener.Properties;
using ApplicationCore;
using ApplicationCore.Code;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
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

            ServerHub.ClientConnected += ServerHub_ClientConnected;
            ServerHub.ClientDisconnected += ServerHub_ClientDisconnected;
            ServerHub.ClientNameChanged += ServerHub_ClientNameChanged;

            ServerHub.ObjReceived += ServerHub_ObjReceived;
        }

        private void ServerHub_ObjReceived(string senderClientId, string obj)
        {
            this.BeginInvoke(new Action(() =>
            {
                _log.Info($"{obj}");
                writeToLog($"{obj}");
            }));
        }

        private void ServerHub_ClientNameChanged(string clientId, string newName)
        {
            this.BeginInvoke(new Action(() =>
            {
                var client = _clients.FirstOrDefault(m => m.Id == clientId);

                if (client != null)
                {
                    client.Name = newName;
                }
            }));

            var content = $"Client name changed. Id:{clientId}, Name:{newName}";
            _log.Info($"{content}");
            writeToLog($"{content}");
        }

        private void ServerHub_ClientDisconnected(string clientId)
        {
            this.BeginInvoke(new Action(() =>
            {
                var client = _clients.FirstOrDefault(m => m.Id == clientId);

                if (client != null)
                {
                    _clients.Remove(client);
                }
            }));

            _log.Info($"Client disconnected: {clientId}");
            writeToLog($"Client disconnected: {clientId}");
        }

        private void ServerHub_ClientConnected(string clientId)
        {
            this.BeginInvoke(new Action(() => _clients.Add(new ClientItem() { Id = clientId, Name = clientId })));

            _log.Info($"Client connected: {clientId}");
            writeToLog($"Client connected: {clientId}");
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

            lvLichSu.EnsureVisible(lvLichSu.Items.Count - 1);
            lvLichSu.Update();
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            lblTrangThai.Text = "Đang kết nối";

            try
            {
                _signalR = WebApp.Start<Startup>(txtServerAIC.Text);

                btnKetNoi.Enabled = false;
                txtServerAIC.Enabled = false;
                btnHuy.Enabled = true;

                lblTrangThai.Text = "Đã kết nối";

                _log.Info($"Server started at: {txtServerAIC.Text}");
                writeToLog($"Server started at: {txtServerAIC.Text}");

                SettingsExtensions.SetValue(txtServerAIC.Text);
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi kết nối";
                _log.Error($"txtKetNoi_Click -> {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            _clients.Clear();

            ServerHub.ClearState();

            if (_signalR != null)
            {
                _signalR.Dispose();
                _signalR = null;

                btnHuy.Enabled = false;
                btnKetNoi.Enabled = true;
                txtServerAIC.Enabled = true;

                lblTrangThai.Text = "Chờ kết nối";

                _log.Info("Server stopped");
                writeToLog("Server stopped");
            }
        }

        private void FrmAICListener_Load(object sender, EventArgs e)
        {
            txtServerAIC.Text = SettingsExtensions.GetValue("AICServerName");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];

            int i = 1;
            int i2 = 1;

            foreach (ListViewItem lvi in lvLichSu.Items)
            {
                i = 1;
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    ws.Cells[i2, i] = lvs.Text;
                    i++;
                }
                i2++;
            }

            var currentPath = Directory.GetCurrentDirectory();

            var excelName = $"{StringHelper.UniqueKey(10)}.xlsx";

            var subPath = $"{currentPath}\\excel";
            if (!System.IO.Directory.Exists(subPath))
            {
                System.IO.Directory.CreateDirectory(subPath);
            }

            var filePath = $"{subPath}\\{excelName}";
            wb.SaveAs(filePath);
            //wb.Close();
            //app.Quit();

            //MessageBox.Show($"Tạo file excel thành công. Bạn có thể tìm tệp theo đường dẫn: {filePath}");
        }
    }
}
