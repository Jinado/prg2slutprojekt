namespace FiaMedKnuff
{
    partial class FrmRules
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRules));
            this.btnBack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblParagraph2 = new System.Windows.Forms.Label();
            this.lblParagraph3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.SlateBlue;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Arial Rounded MT Bold", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(1023, 542);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(315, 59);
            this.btnBack.TabIndex = 5;
            this.btnBack.Text = "TILLBAKA";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnBack.MouseEnter += new System.EventHandler(this.btnBack_MouseEnter);
            this.btnBack.MouseLeave += new System.EventHandler(this.btnBack_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1204, 112);
            this.label1.TabIndex = 6;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(546, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 43);
            this.label2.TabIndex = 7;
            this.label2.Text = "SPELREGLER";
            // 
            // lblParagraph2
            // 
            this.lblParagraph2.AutoSize = true;
            this.lblParagraph2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParagraph2.Location = new System.Drawing.Point(30, 224);
            this.lblParagraph2.Name = "lblParagraph2";
            this.lblParagraph2.Size = new System.Drawing.Size(1185, 140);
            this.lblParagraph2.TabIndex = 8;
            this.lblParagraph2.Text = resources.GetString("lblParagraph2.Text");
            // 
            // lblParagraph3
            // 
            this.lblParagraph3.AutoSize = true;
            this.lblParagraph3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParagraph3.Location = new System.Drawing.Point(30, 396);
            this.lblParagraph3.Name = "lblParagraph3";
            this.lblParagraph3.Size = new System.Drawing.Size(1153, 56);
            this.lblParagraph3.TabIndex = 9;
            this.lblParagraph3.Text = "Den som vinner spelet är den som först kommer in i målet med alla fyra av sina pj" +
    "äser. Målet är den\r\nsvarta brickan i mitten av spelplanen.";
            // 
            // FrmRules
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1350, 613);
            this.Controls.Add(this.lblParagraph3);
            this.Controls.Add(this.lblParagraph2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmRules";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fia med knuff - Johannes Emmoth";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblParagraph2;
        private System.Windows.Forms.Label lblParagraph3;
    }
}
