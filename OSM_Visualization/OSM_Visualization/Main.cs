using System.Windows.Forms;
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

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            bitmap = new Bitmap(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height - 50);

            dbPanel1.Width = bitmap.Width;
            dbPanel1.Height = bitmap.Height;
            dbPanel1.Location = new Point(0, 50);

            DrawButton.Location = new Point(bitmap.Width / 2 - DrawButton.Width, 10);
           
        }

        void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileArgs = (string[])e.Data.GetData(DataFormats.FileDrop);
            fileLoc = fileArgs[0];
        }

        void Draw_buttonClick(object sender, EventArgs e)
        {
            DrawButton.Enabled = false;
            LoadAndDraw();
        }

        async void LoadAndDraw()
        {
            MapDrawer drawer;
            OSMDataManager xmlData;

            xmlData = await Task.Run(() => new OSMDataManager(fileLoc));

            drawer = new MapDrawer(dbPanel1, ref bitmap);

            await Task.Run(() => Draw(ref xmlData, drawer));

            await Task.Run(() => drawer.Dispose());
            await Task.Run(() => xmlData.Dispose());

            GC.Collect();
            dbPanel1.Invalidate();
            DrawButton.Enabled = true;
        }

        void Draw(ref OSMDataManager xmlData, MapDrawer drawer) => drawer.DrawMap(ref xmlData);

        private void Main_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawImage(bitmap, Point.Empty);
    }
}
