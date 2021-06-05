using ApplicationCore;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
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
                grpSend.Enabled = true;

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
            if (_signalRConnection != null)
            {
                _signalRConnection.Stop();
                _signalRConnection.Dispose();
                _signalRConnection = null;

                btnKetNoi.Enabled = true;
                btnHuy.Enabled = false;

                lblTrangThai.Text = "Chờ kết nối";
                grpSend.Enabled = false;
            }
        }

        private void btn1Gui_Click(object sender, EventArgs e)
        {
            lbl1ThongBao.Text = "";
            var trangThai = rd1DangKy.Checked ? "Đăng ký" : (rd1Huy.Checked ? "Hủy" : "");
            var soGhe = txtGhe.Text;

            if (string.IsNullOrEmpty(soGhe) || string.IsNullOrEmpty(trangThai))
            {
                lbl1ThongBao.Text = "Vui lòng chọn số ghế và trạng thái";
            }
            else
            {
                var objMessage = new ObjMessage()
                {
                    AICSenderFake = "AIC Sender Fake Name",
                    SoGhe = soGhe,
                    TrangThai = trangThai
                };

                var objMessageStr = JsonConvert.SerializeObject(objMessage);

                _hubProxy.Invoke("SendObj", objMessageStr);
            }
        }

        private async void btnnGui_Click(object sender, EventArgs e)
        {
            lblnThongBao.Text = "";

            int soLenh = Convert.ToInt32(numLenh.Value);

            if (soLenh <= 0)
            {
                lblnThongBao.Text = "Số lệnh phải lớn hơn 0";
            }
            else
            {
                int oldNumber = -1;
                for (int i = 1; i <= soLenh; i++)
                {
                    var trangthai = string.Empty;

                    if (rdnDangKy.Checked)
                    {
                        trangthai = "Đăng ký";
                    }
                    else if (rdnHuy.Checked)
                    {
                        trangthai = "Hủy";
                    }
                    else if (rdnNgauNhien.Checked)
                    {
                        var rand = new Random();
                        var randNumber = rand.Next(1, 100);
                        if (randNumber % 2 == 0)
                        {
                            trangthai = "Đăng ký";
                        }
                        else
                        {
                            trangthai = "Hủy";
                        }
                    }

                    var randGheNumber = GetNewRandom(oldNumber);
                    oldNumber = randGheNumber;

                    var objMessage = new ObjMessage()
                    {
                        AICSenderFake = "AIC Sender Fake Name",
                        SoGhe = $"ABC{randGheNumber}",
                        TrangThai = trangthai
                    };

                    var objMessageStr = JsonConvert.SerializeObject(objMessage);

                    await _hubProxy.Invoke("SendObj", objMessageStr);

                }
            }

        }

        private int GetNewRandom(int oldNumber)
        {
            var random = new Random();
            var randomGhe = random.Next(1, 1000);

            if (randomGhe != oldNumber)
            {
                return randomGhe;
            }
            else
            {
                return GetNewRandom(oldNumber);
            }
        }
    }
}
