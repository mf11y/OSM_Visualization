﻿using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Threading;

namespace OSM_Visualization
{
    public partial class MainWindow : Form
    {
        Bitmap bitmap;
        string fileLoc;

        MapDrawer drawer;
        OSMDataManager xmlData;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Height = Screen.GetWorkingArea(this).Height;
            this.Width = Screen.GetWorkingArea(this).Width;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            bitmap = new Bitmap(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height - 50);

            dbPanel1.Width = bitmap.Width;
            dbPanel1.Height = bitmap.Height;
            dbPanel1.Location = new Point(0, 50);

            DrawButton.Location = new Point((bitmap.Width / 2) - DrawButton.Width / 2, 10);
            trackBar1.Location = new Point((bitmap.Width / 2) - trackBar1.Width / 2, bitmap.Height - 50);
            textBox1.Location = new Point((bitmap.Width / 2) - textBox1.Width / 2, (bitmap.Height / 2) - textBox1.Height);

        }

        void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void Main_DragDrop(object sender, DragEventArgs e)
        {
            DrawButton.Enabled = true;
            string[] fileArgs = (string[])e.Data.GetData(DataFormats.FileDrop);
            fileLoc = fileArgs[0];
        }

        void Draw_buttonClick(object sender, EventArgs e)
        {

            textBox1.Visible = true;
            DrawButton.Enabled = false;
            LoadAndDraw();
        }

        async void LoadAndDraw()
        {

            xmlData = await Task.Run(() => new OSMDataManager(fileLoc));

            drawer = new MapDrawer(new Tuple<int, int>(bitmap.Width, bitmap.Height));

            bitmap = new Bitmap (await Task.Run(() => Draw(ref xmlData, drawer)));

            //await Task.Run(() => drawer.Dispose());
            //await Task.Run(() => xmlData.Dispose());

            GC.Collect();
            textBox1.Visible = false;
            trackBar1.Enabled = true;
            RefreshScreen();
        }

        Bitmap Draw(ref OSMDataManager xmlData, MapDrawer drawer) => drawer.DrawMap(ref xmlData);

        private void Main_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawImage(bitmap, Point.Empty);

        private void RefreshScreen()
        {
            dbPanel1.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            if (trackBar1.Value % 5 == 0)
            {
                xmlData.ZoomBounds(trackBar1.Value);
                bitmap = new Bitmap(drawer.DrawMap(ref xmlData));
                RefreshScreen();
            }
        }
    }
}
