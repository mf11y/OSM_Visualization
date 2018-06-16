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
    public partial class Main1 : Form
    {

        OSMDataManager xmlData;
        MapDrawer drawer;
        Bitmap bitmap;

        public Main1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            bitmap = new Bitmap(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);

        }

        void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        async void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileLoc = (string[])e.Data.GetData(DataFormats.FileDrop);

            await Task.Run(() => GetData(fileLoc[0])); 

            drawer = new MapDrawer(dbPanel1, ref bitmap);
            await Task.Run(() => Draw());

            xmlData = null;
            drawer = null;
        }

        void GetData(string fileLoc) => xmlData = new OSMDataManager(fileLoc);

        void Draw() => drawer.DrawMap(ref xmlData);

        private void Main_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawImage(bitmap, Point.Empty);

    }
}
