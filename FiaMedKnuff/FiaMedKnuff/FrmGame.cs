using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmGame : Form
    {
        public FrmGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This methods handles the message recieved from the server
        /// </summary>
        public void HandleMessageRecievedByServer(string message)
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
            }
        }
    }
}
