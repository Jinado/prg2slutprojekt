namespace FiaMedKnuff
{
    partial class FrmMain
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnRules = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FiaMedKnuff.Properties.Resources.title;
            this.pictureBox1.Location = new System.Drawing.Point(33, 48);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(465, 43);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::FiaMedKnuff.Properties.Resources.bakgrundsbild;
            this.pictureBox2.Location = new System.Drawing.Point(536, 109);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(748, 392);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.SlateBlue;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Font = new System.Drawing.Font("Arial Rounded MT Bold", 36.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlay.ForeColor = System.Drawing.Color.White;
            this.btnPlay.Location = new System.Drawing.Point(72, 209);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(369, 63);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.Text = "SPELA";
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnPlay.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnRules
            // 
            this.btnRules.BackColor = System.Drawing.Color.SlateBlue;
            this.btnRules.FlatAppearance.BorderSize = 0;
            this.btnRules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRules.Font = new System.Drawing.Font("Arial Rounded MT Bold", 36.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRules.ForeColor = System.Drawing.Color.White;
            this.btnRules.Location = new System.Drawing.Point(72, 310);
            this.btnRules.Name = "btnRules";
            this.btnRules.Size = new System.Drawing.Size(369, 63);
            this.btnRules.TabIndex = 3;
            this.btnRules.Text = "SPELREGLER";
            this.btnRules.UseVisualStyleBackColor = false;
            this.btnRules.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnRules.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.SlateBlue;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Arial Rounded MT Bold", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(102, 407);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(315, 59);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "AVLSUTA";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            this.btnExit.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnExit.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1350, 613);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRules);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fia Med Knuff av Johannes Emmoth";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnRules;
        private System.Windows.Forms.Button btnExit;
    }
}

