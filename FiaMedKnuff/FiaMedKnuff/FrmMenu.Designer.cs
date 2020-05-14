namespace FiaMedKnuff
{
    partial class FrmMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbxHost = new System.Windows.Forms.PictureBox();
            this.pbxPlay = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ltbHostName = new LabeledTextBox();
            this.ltbMaxPlayers = new LabeledTextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbxHost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlay)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxHost
            // 
            this.pbxHost.Image = global::FiaMedKnuff.Properties.Resources.border;
            this.pbxHost.Location = new System.Drawing.Point(12, 12);
            this.pbxHost.Name = "pbxHost";
            this.pbxHost.Size = new System.Drawing.Size(589, 589);
            this.pbxHost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxHost.TabIndex = 0;
            this.pbxHost.TabStop = false;
            // 
            // pbxPlay
            // 
            this.pbxPlay.Image = global::FiaMedKnuff.Properties.Resources.border;
            this.pbxPlay.Location = new System.Drawing.Point(749, 12);
            this.pbxPlay.Name = "pbxPlay";
            this.pbxPlay.Size = new System.Drawing.Size(589, 589);
            this.pbxPlay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxPlay.TabIndex = 1;
            this.pbxPlay.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(163, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "HÅLL I ETT SPEL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(822, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(458, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "GÅ MED I NÅGON ANNAS SPEL";
            // 
            // ltbHostName
            // 
            this.ltbHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbHostName.ForeColor = System.Drawing.Color.Gray;
            this.ltbHostName.Label = "Ditt namn...";
            this.ltbHostName.Location = new System.Drawing.Point(66, 96);
            this.ltbHostName.Name = "ltbHostName";
            this.ltbHostName.Size = new System.Drawing.Size(177, 26);
            this.ltbHostName.TabIndex = 4;
            this.ltbHostName.Text = "Ditt namn...";
            // 
            // ltbMaxPlayers
            // 
            this.ltbMaxPlayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbMaxPlayers.ForeColor = System.Drawing.Color.Gray;
            this.ltbMaxPlayers.Label = "Max antal spelare (2-4)";
            this.ltbMaxPlayers.Location = new System.Drawing.Point(66, 135);
            this.ltbMaxPlayers.Name = "ltbMaxPlayers";
            this.ltbMaxPlayers.Size = new System.Drawing.Size(177, 26);
            this.ltbMaxPlayers.TabIndex = 5;
            this.ltbMaxPlayers.Text = "Max antal spelare (2-4)";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.SlateBlue;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(299, 128);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(232, 36);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "STARTA SERVERN";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnExit.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(442, 96);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(89, 26);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "6767";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(376, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 24);
            this.label3.TabIndex = 9;
            this.label3.Text = "Port:";
            // 
            // FrmMenu
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1350, 613);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.ltbMaxPlayers);
            this.Controls.Add(this.ltbHostName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbxPlay);
            this.Controls.Add(this.pbxHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fia med knuff - Johannes Emmoth";
            ((System.ComponentModel.ISupportInitialize)(this.pbxHost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxHost;
        private System.Windows.Forms.PictureBox pbxPlay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private LabeledTextBox ltbHostName;
        private LabeledTextBox ltbMaxPlayers;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
    }
}
