namespace OSM_Visualization
{
    partial class Main
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
            this.dbPanel1 = new OSM_Visualization.DBPanel();
            this.SuspendLayout();
            // 
            // dbPanel1
            // 
            this.dbPanel1.Location = new System.Drawing.Point(82, 116);
            this.dbPanel1.Name = "dbPanel1";
            this.dbPanel1.Size = new System.Drawing.Size(500, 500);
            this.dbPanel1.TabIndex = 0;
            this.dbPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.dbPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.dbPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(684, 617);
            this.Controls.Add(this.dbPanel1);
            this.Name = "Main";
            this.Text = "Map Visualization";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private DBPanel dbPanel1;
    }
}

