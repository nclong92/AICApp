using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AICSenderFake
{
    public partial class FrmAICSenderFake : Form
    {
        HubConnection _signalRConnection;
        IHubProxy _hubProxy;

        public FrmAICSenderFake()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private async void btnKetNoi_Click(object sender, EventArgs e)
        {
            _signalRConnection = new HubConnection(txtServerAIC.Text);
            _signalRConnection.StateChanged += HubConnection_StateChanged;

            _hubProxy = _signalRConnection.CreateHubProxy("ServerHub");

            _hubProxy.On<string, string>("AddObjMessage", (name, message) =>
            {
                // LongNC TODO: write log when it need

            });

            btnKetNoi.Enabled = false;

            try
            {
                await _signalRConnection.Start();
                await _hubProxy.Invoke("SetUserName", "AIC Sender Fake");

                txtServerAIC.Enabled = false;
                btnHuy.Enabled = true;

            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi kết nối";
                btnKetNoi.Enabled = true;
            }
        }

        private void HubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                lblTrangThai.Text = "Đã kết nối";
            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                lblTrangThai.Text = "Chờ kết nối";
            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connecting)
            {
                lblTrangThai.Text = "Đang kết nối";
            }
            //else
            //{
            //    lblTrangThai.Text = "Lỗi kết nối";
            //}
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if(_signalRConnection != null)
            {
                _signalRConnection.Stop();
                _signalRConnection.Dispose();
                _signalRConnection = null;

                btnKetNoi.Enabled = true;
                btnHuy.Enabled = false;

                lblTrangThai.Text = "Chờ kết nối";
            }
        }
    }
}
