using System.Windows.Forms;
using System.Drawing;
using System;
using System.Threading.Tasks;

namespace OSM_Visualization
{
    public partial class MainWindow : Form
    {
        Bitmap bitmap;
        string fileLoc;

        MapDrawer drawer;
        OSMDataManager xmlData;

        Bitmap fullSizedBitmap;
        Bitmap mediumSizedBitmap;
        Bitmap fullSizedBitmapResize;
        Bitmap mediumSizedBitmapResize;


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
            textBox1.Location = new Point((bitmap.Width / 2) - textBox1.Width / 2, (bitmap.Height / 2) - textBox1.Height);

            bitmap = new Bitmap(Screen.GetWorkingArea(this).Width * 4, (Screen.GetWorkingArea(this).Height - 50) * 4);
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
            textBox1.Text = "Loading in Nodes...";
            DrawButton.Enabled = false;
            LoadAndDraw();
        }

        async void LoadAndDraw()
        {

            xmlData = await Task.Run(() => new OSMDataManager(fileLoc));

            drawer = new MapDrawer(new Tuple<int, int>(bitmap.Width, bitmap.Height));

            textBox1.Text = "Drawing Map...";
            fullSizedBitmap = new Bitmap (await Task.Run(() => Draw(ref xmlData, drawer)));

            await Task.Run(() => drawer.Dispose());
            await Task.Run(() => xmlData.Dispose());

            xmlData = null;
            drawer = null;

            CreateBitmaps();

            GC.Collect();
            GC.Collect();
            RefreshScreen();
            textBox1.Visible = false;
        }

        private void CreateBitmaps()
        {
            mediumSizedBitmap = new Bitmap(fullSizedBitmap, 3840, 2160);
            mediumSizedBitmapResize = new Bitmap(mediumSizedBitmap, 1920, 1080);
            fullSizedBitmapResize = new Bitmap(fullSizedBitmap, 1920, 1080);
        }

        Bitmap Draw(ref OSMDataManager xmlData, MapDrawer drawer) => drawer.DrawMap(ref xmlData);

        private void RefreshScreen()
        {
            bitmap = new Bitmap(fullSizedBitmap, 1920, 1080);
            pictureBox1.Image = bitmap;
        }

        int zoomFactor = 0;

        private void PictureBox1_Click(object sender, MouseEventArgs e)
        {

            int x = pictureBox1.PointToClient(Cursor.Position).X;
            int y = pictureBox1.PointToClient(Cursor.Position).Y;

            if(e.Button == MouseButtons.Left)
            {
                zoomFactor++;
                if (zoomFactor == 1)
                {
                    if (x / 960 == 0)
                    {
                        pictureBox1.Image = mediumSizedBitmap;
                    }
                    else if (x / 960 == 1)
                    {
                        bitmap = mediumSizedBitmap;
                        pictureBox1.Image = mediumSizedBitmap;
                        pictureBox1.Location = new Point(-1 * (bitmap.Width / 2), 0);
                    }
                }
            }
            else
            {
                if (zoomFactor == 1)
                {
                    pictureBox1.Image = fullSizedBitmapResize;
                    pictureBox1.Location = new Point(0, 0);
                }
                zoomFactor--;
            }
        }
    }
}
