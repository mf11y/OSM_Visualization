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
            this.ZoomButton = new System.Windows.Forms.Button();
            this.dbPanel1 = new OSM_Visualization.DBPanel();
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
            // ZoomButton
            // 
            this.ZoomButton.Location = new System.Drawing.Point(712, 10);
            this.ZoomButton.Name = "ZoomButton";
            this.ZoomButton.Size = new System.Drawing.Size(172, 32);
            this.ZoomButton.TabIndex = 2;
            this.ZoomButton.Text = "Zoom";
            this.ZoomButton.UseVisualStyleBackColor = true;
            this.ZoomButton.Click += new System.EventHandler(this.ZoomButton_Click);
            // 
            // dbPanel1
            // 
            this.dbPanel1.AllowDrop = true;
            this.dbPanel1.BackColor = System.Drawing.SystemColors.GrayText;
            this.dbPanel1.Location = new System.Drawing.Point(0, 45);
            this.dbPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.dbPanel1.Name = "dbPanel1";
            this.dbPanel1.Size = new System.Drawing.Size(994, 540);
            this.dbPanel1.TabIndex = 0;
            this.dbPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.dbPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.dbPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(994, 585);
            this.Controls.Add(this.ZoomButton);
            this.Controls.Add(this.DrawButton);
            this.Controls.Add(this.dbPanel1);
            this.Name = "MainWindow";
            this.Text = "Map Visualization";
            this.ResumeLayout(false);

        }

        #endregion

        private DBPanel dbPanel1;
        private System.Windows.Forms.Button DrawButton;
        private System.Windows.Forms.Button ZoomButton;
    }
}

