using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmRules : Form
    {
        public FrmRules()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FrmMain().ShowDialog();
            this.Close();
        }

        private void btnBack_MouseEnter(object sender, EventArgs e)
        {
            btnBack.BackColor = Color.IndianRed;
            btnBack.Size = new Size(btnBack.Size.Width + 10, btnBack.Size.Height + 10);
            btnBack.Left -= 5;
            btnBack.Top -= 5;
        }

        private void btnBack_MouseLeave(object sender, EventArgs e)
        {
            btnBack.BackColor = Color.SlateBlue;
            btnBack.Size = new Size(btnBack.Size.Width - 10, btnBack.Size.Height - 10);
            btnBack.Left += 5;
            btnBack.Top += 5;
        }
    }
}
