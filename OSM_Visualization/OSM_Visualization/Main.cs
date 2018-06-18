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

            this.Cursor = Cursors.SizeAll;

           
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

            DrawButton.Enabled = false;
            Font FontDraw = new Font("Arial", 56);

            Graphics gr;
            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            SolidBrush drawBrush = new SolidBrush(Color.White);

            gr.Clear(Color.Gray);
            dbPanel1.Invalidate();

            gr.DrawString("Loading...", FontDraw, drawBrush, bitmap.Width / 2 - 125, bitmap.Height / 2 - 50);

            dbPanel1.Invalidate();


            LoadAndDraw();
        }

        async void LoadAndDraw()
        {

            xmlData = await Task.Run(() => new OSMDataManager(fileLoc));

            drawer = new MapDrawer(dbPanel1, ref bitmap);

            await Task.Run(() => Draw(ref xmlData, drawer));

            //await Task.Run(() => drawer.Dispose());
            //await Task.Run(() => xmlData.Dispose());

            GC.Collect();
            dbPanel1.Invalidate();
        }

        void Draw(ref OSMDataManager xmlData, MapDrawer drawer) => drawer.DrawMap(ref xmlData, 0);

        private void Main_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawImage(bitmap, Point.Empty);

        private void ZoomButton_Click(object sender, EventArgs e)
        {
            drawer.DrawMap(ref xmlData, 1);
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            drawer.DrawMap(ref xmlData, 2);
        }
    }
}
