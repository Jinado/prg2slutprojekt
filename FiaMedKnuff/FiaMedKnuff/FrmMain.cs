using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmMain : Form
    {
        Server host;
        Server server;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.IndianRed;
            btn.Size = new Size(btn.Size.Width + 10, btn.Size.Height + 10);
            btn.Left -= 5;
            btn.Top -= 5;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.SlateBlue;
            btn.Size = new Size(btn.Size.Width - 10, btn.Size.Height - 10);
            btn.Left += 5;
            btn.Top += 5;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FrmMenu().ShowDialog();
            this.Close();
        }

        private void btnRules_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FrmRules().ShowDialog();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLaunchServer_Click(object sender, EventArgs e)
        {
            //host = new Server();
            Server.StartServer(host);
            MessageBox.Show("Server started");
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient();
            client.NoDelay = true;
            server = new Server(client, tbxIP.Text);

            if (await Server.JoinServer(server))
                MessageBox.Show("Succesfully joined the server");
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            Server.ThrownDice(server, 6);
        }
    }
}
