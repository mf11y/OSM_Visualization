namespace OSM_Visualization
{
    partial class MainWindow
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
            this.DrawButton = new System.Windows.Forms.Button();
            this.dbPanel1 = new OSM_Visualization.DBPanel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dbPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // DrawButton
            // 
            this.DrawButton.Enabled = false;
            this.DrawButton.Location = new System.Drawing.Point(381, 10);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new System.Drawing.Size(172, 32);
            this.DrawButton.TabIndex = 1;
            this.DrawButton.Text = "Draw";
            this.DrawButton.UseVisualStyleBackColor = true;
            this.DrawButton.Click += new System.EventHandler(this.Draw_buttonClick);
            // 
            // dbPanel1
            // 
            this.dbPanel1.AllowDrop = true;
            this.dbPanel1.BackColor = System.Drawing.SystemColors.GrayText;
            this.dbPanel1.Controls.Add(this.textBox1);
            this.dbPanel1.Controls.Add(this.trackBar1);
            this.dbPanel1.Location = new System.Drawing.Point(0, 45);
            this.dbPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.dbPanel1.Name = "dbPanel1";
            this.dbPanel1.Size = new System.Drawing.Size(994, 540);
            this.dbPanel1.TabIndex = 0;
            this.dbPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.dbPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.dbPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            // 
            // trackBar1
            // 
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(350, 495);
            this.trackBar1.Maximum = 75;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(255, 45);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TabStop = false;
            this.trackBar1.TickFrequency = 25;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.GrayText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(332, 201);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(337, 76);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Loading. . .";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(994, 585);
            this.Controls.Add(this.DrawButton);
            this.Controls.Add(this.dbPanel1);
            this.Name = "MainWindow";
            this.Text = "Map Visualization";
            this.dbPanel1.ResumeLayout(false);
            this.dbPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DBPanel dbPanel1;
        private System.Windows.Forms.Button DrawButton;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

