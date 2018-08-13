using System.Windows.Forms;
using System.Drawing;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OSM_Visualization
{
    public partial class MainWindow : Form
    {
        string fileLoc;

        MapDrawer drawer;
        OSMDataManager xmlData;

        Bitmap fullSizedBitmap;
        Bitmap mediumSizedBitmap;
        Bitmap fullSizedBitmapResize;


        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Height = Screen.GetWorkingArea(this).Height;
            this.Width = Screen.GetWorkingArea(this).Width;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            dbPanel1.Width = Screen.GetWorkingArea(this).Width;
            dbPanel1.Height = Screen.GetWorkingArea(this).Height - 50;
            dbPanel1.Location = new Point(0, 50);

            DrawButton.Location = new Point((dbPanel1.Width / 2) - DrawButton.Width / 2, 10);
            textBox1.Location = new Point((dbPanel1.Width / 2) - textBox1.Width / 2, (dbPanel1.Height / 2) - textBox1.Height);

            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

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
            textBox1.Text = "Loading in Nodes...";
            xmlData = await Task.Run(() => new OSMDataManager(fileLoc));

            textBox1.Text = "Drawing Map...";
            drawer = new MapDrawer(new Tuple<int, int>(dbPanel1.Width * 4, dbPanel1.Height * 4));
            fullSizedBitmap = new Bitmap (await Task.Run(() => drawer.DrawMap(ref xmlData)));

            await Task.Run(() => drawer.Dispose());
            await Task.Run(() => xmlData.Dispose());

            CreateBitmaps();

            GC.Collect();
            GC.Collect();
            RefreshScreen();
            textBox1.Visible = false;
        }

        private void CreateBitmaps()
        {
            mediumSizedBitmap = new Bitmap(fullSizedBitmap, dbPanel1.Width*2, dbPanel1.Height*2);
            fullSizedBitmapResize = new Bitmap(fullSizedBitmap, dbPanel1.Width, dbPanel1.Height);
        }


        private void RefreshScreen()
        {
            pictureBox1.Image = fullSizedBitmapResize;
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        int zoomFactor = 0;
        const int zoomLimit = 2;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (zoomFactor < zoomLimit)
                {
                    int x = e.X;
                    int y = e.Y;

                    Point incoming = new Point(x, y);
                    zoomFactor++;

                    int xTransformed = 0;
                    int yTransformed = 0;

                    int zoomWidth;
                    int zoomHeight;


                    if (zoomFactor == 1)
                    {
                        pictureBox1.Image = mediumSizedBitmap;
                        zoomWidth = mediumSizedBitmap.Width;
                        zoomHeight = mediumSizedBitmap.Height;
                    }
                    else
                    {
                        pictureBox1.Image = fullSizedBitmap;
                        zoomWidth = fullSizedBitmap.Width;
                        zoomHeight = fullSizedBitmap.Height;
                    }

                    xTransformed = x * 2;
                    xTransformed -= dbPanel1.Width / 2;

                    yTransformed = y * 2;
                    yTransformed -= dbPanel1.Height / 2;


                    if (xTransformed + (dbPanel1.Width / 2) > zoomWidth - dbPanel1.Width / 2)
                        xTransformed = zoomWidth - dbPanel1.Width;
                    else if (xTransformed < 0)
                        xTransformed = 0;

                    if (yTransformed + (dbPanel1.Height / 2) > zoomHeight - dbPanel1.Height / 2)
                        yTransformed = zoomHeight - dbPanel1.Height;
                    else if (yTransformed < 0)
                        yTransformed = 0;

                    pictureBox1.Location = new Point(-1 * xTransformed, -1 * yTransformed);
                }
            }
            else
            {
                if(zoomFactor == 1)
                {
                    pictureBox1.Image = fullSizedBitmapResize;
                    pictureBox1.Location = new Point(0, 0);
                    zoomFactor--;
                }
                else if(zoomFactor == 2)
                {
                    pictureBox1.Image = mediumSizedBitmap;

                    int x = (pictureBox1.Location.X - (dbPanel1.Width/2)) / 2;
                    int y = (pictureBox1.Location.Y - (dbPanel1.Height/2)) / 2;

                    x += dbPanel1.Width / 2;
                    y += dbPanel1.Height / 2;

                    if (x - dbPanel1.Width < -1 * mediumSizedBitmap.Width)
                        x = (mediumSizedBitmap.Width - dbPanel1.Width) * -1;
                    else if (x > 0)
                        x = 0;

                    if (y - dbPanel1.Height < -1 * mediumSizedBitmap.Height)
                        y = (mediumSizedBitmap.Height - dbPanel1.Height) * -1;
                    else if (y > 0)
                        y = 0;

                    pictureBox1.Location = new Point(x, y);
                    zoomFactor--;
                }
            }
        }

        bool mouseDown;

        Point mouseDownPoint = new Point();

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownPoint.X = e.X;
            mouseDownPoint.Y = e.Y;
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(mouseDown && zoomFactor!= 0 )
            {
                int x, y;

                x = pictureBox1.Location.X - (mouseDownPoint.X - e.X);
                y = pictureBox1.Location.Y - (mouseDownPoint.Y - e.Y);

                if (!(x < 0 && x > pictureBox1.Width * -1 + 1920))
                    x = pictureBox1.Location.X;

                if (!(y < 0 && y > pictureBox1.Height * -1 + 1080))
                    y = pictureBox1.Location.Y;

               pictureBox1.Location = new Point(x, y);

            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
