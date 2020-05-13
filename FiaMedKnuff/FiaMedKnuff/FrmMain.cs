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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {

        }

        private void btnLaunchServer_Click(object sender, EventArgs e)
        {
            int port = 6767;
            int maxPlayers = 4;
            host = new Server(port, maxPlayers);
            Server.StartServer(host);
            MessageBox.Show("Server started");
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            int port = 6767;
            TcpClient client = new TcpClient();
            client.NoDelay = true;
            server = new Server(client, tbxIP.Text, port);

            if (await Server.JoinServer(server))
                MessageBox.Show("Succesfully joined the server");
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            Server.ThrownDice(server, 6);
        }

        /// <summary>
        /// This methods handles the message recieved from the server
        /// </summary>
        public static void HandleMessageRecievedByServer(string message)
        {
            string msgType = $"{message[0]}{message[1]}{message[2]}";

            switch (msgType)
            {
                case "PLD": // Player disconnected
                    break;
                case "MVC": // A character has been moved
                    break;
                case "HAW": // A player has won
                    break;
                case "TRD": // The dice have been thrown
                    break;
                case "CHT": // The turn has been changed
                    break;
                case "SPD": // Player data has been sent
                    break;
                case "SRS": // Ready status of all players have been sent
                    break;
            }
        }
    }
}
