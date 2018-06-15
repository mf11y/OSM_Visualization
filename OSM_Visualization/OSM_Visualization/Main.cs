using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace OSM_Visualization
{
    public partial class Main : Form
    {

        OSMDataManager loader;
        MapDrawer drawer;
        Bitmap bitmap;

        public Main()
        {
            InitializeComponent();
            bitmap = new Bitmap(dbPanel1.Height, dbPanel1.Width);
            //bitmap.Save(@"c:\temp\bmap.bmp");

        }

        void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileLoc = (string[])e.Data.GetData(DataFormats.FileDrop);
            loader = new OSMDataManager(fileLoc[0]);
            drawer = new MapDrawer(dbPanel1, ref loader, ref bitmap);

            draw();

        }

        async void draw()
        {
            await Task.Run(() => drawer.DrawMap());


            dbPanel1.Invalidate();
        }


        private void Main_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.DrawImage(bitmap, Point.Empty);

        }
    }
}
