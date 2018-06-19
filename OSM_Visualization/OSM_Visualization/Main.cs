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
        Bitmap smallSizedBitmap;

        Bitmap fullSizedBitmapResize;
        Bitmap mediumSizedBitmapResize;
        Bitmap smallSizedBitmapResize;


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
            drawer = new MapDrawer(new Tuple<int, int>(1920 * 4, 1000 * 4));
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
            mediumSizedBitmap = new Bitmap(fullSizedBitmap, 3840, 2000);
            mediumSizedBitmapResize = new Bitmap(mediumSizedBitmap, 1920, 1000);

            smallSizedBitmap = new Bitmap(mediumSizedBitmap, 1920, 1000);


            fullSizedBitmapResize = new Bitmap(fullSizedBitmap, 1920, 1000);
        }


        private void RefreshScreen()
        {
            pictureBox1.Image = fullSizedBitmapResize;
        }

        int zoomFactor = 0;

        Stack<Point> Moves = new Stack<Point>();

        private void PictureBox1_Click(object sender, MouseEventArgs e)
        {

            int x = dbPanel1.PointToClient(Cursor.Position).X;
            int y = dbPanel1.PointToClient(Cursor.Position).Y;

            if(e.Button == MouseButtons.Left)
            {
                Point incoming = new Point(x / 960, y/500);
                zoomFactor++;

                if (zoomFactor == 1)
                {
                    pictureBox1.Image = mediumSizedBitmap;

                    if (incoming.X == 1 && incoming.Y == 0)
                    {
                        pictureBox1.Location = new Point(-1 * (mediumSizedBitmap.Width / 2), 0);
                    }
                    else if(incoming.X == 0 && incoming.Y == 1)
                    {
                        pictureBox1.Location = new Point(0, -1 * (mediumSizedBitmap.Height/2));
                    }
                    else if(incoming.X == 1 && incoming.Y == 1)
                    {
                        pictureBox1.Location = new Point(-1 * (mediumSizedBitmap.Width / 2), -1 * (mediumSizedBitmap.Height / 2));
                    }

                    Moves.Push(incoming);
                }
                if(zoomFactor == 2)
                {
                    Point oldMove = Moves.Peek();

                    Point adjQuad = new Point() ;

                    if(oldMove.X == 0 && oldMove.Y == 0)
                    {
                        adjQuad.X = incoming.X;
                        adjQuad.Y = incoming.Y;
                    }
                    if (oldMove.X == 0 && oldMove.Y == 1)
                    {
                        adjQuad.X = incoming.X;
                        adjQuad.Y = incoming.Y + 2;

                    }
                    if (oldMove.X == 1 && oldMove.Y == 0)
                    {
                        adjQuad.X = incoming.X + 2;
                        adjQuad.Y = incoming.Y;
                    }
                    if (oldMove.X == 1 && oldMove.Y == 1)
                    {
                        adjQuad.X = incoming.X + 2;
                        adjQuad.Y = incoming.Y + 2;
                    }

                    pictureBox1.Image = fullSizedBitmap;
                    pictureBox1.Location = new Point(-1 * (1920 * adjQuad.X), -1 * (1000 * adjQuad.Y));
                }
            }
            else
            {

                    pictureBox1.Image = fullSizedBitmapResize;
                    pictureBox1.Location = new Point(0, 0);
                    zoomFactor = 0; ;
                    Moves.Pop();

            }
        }
    }
}
