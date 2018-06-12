using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace OSM_Visualization
{
    public partial class Main : Form
    {

        XDocument osmFile = new XDocument();

        float firstPointLat;
        float firstPointLon;
        float secondPointLat;
        float secondPointLon;

        float minLat;
        float minLon;

        float maxLat;
        float maxLon;

        public Main()
        {
            InitializeComponent();
        }

        void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            osmFile = XDocument.Load(files[0]);

            minLat = float.Parse(osmFile.Root.Element("bounds").Attribute("minlat").Value);
            minLon = float.Parse(osmFile.Root.Element("bounds").Attribute("minlon").Value);
            maxLat = float.Parse(osmFile.Root.Element("bounds").Attribute("maxlat").Value);
            maxLon = float.Parse(osmFile.Root.Element("bounds").Attribute("maxlon").Value);



            //var query = osmFile.Descendants("way").Select(item => item.Element("nd"));
            var query = osmFile.Descendants("way").Select(x => x.Elements("nd").Select(z => z.Attribute("ref").Value));

            int current = 0;
            foreach (var x in query)
            {
                //XDocument ea = XDocument.Parse(x.ToString());

                current = 0;

                foreach(var y in x.ToList())
                {
                    if (current == 0)
                    {
                        firstPointLat = float.Parse(osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == y).Attribute("lat").Value);
                        firstPointLon = float.Parse(osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == y).Attribute("lon").Value);
                        current++;
                    }
                    else
                    {
                        secondPointLat = float.Parse(osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == y).Attribute("lat").Value);
                        secondPointLon = float.Parse(osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == y).Attribute("lon").Value);
                        current--;
                    }

                    dbPanel1.Refresh();
                }

                //MessageBox.Show(string.Join("\n", x));    
            }
        }

        private static readonly Pen myPen = new Pen(Brushes.Red, 1);

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;

            float normalizedp1Lat = ((firstPointLat - minLat) / (maxLat - minLat)) * 500;
            float normalizedp1Lon = ((firstPointLon - minLon) / (maxLon - minLon)) * 500;

            float normalizedp2Lat = ((secondPointLat - minLat) / (maxLat - minLat)) * 500;
            float normalizedp2Lon = ((secondPointLon - minLon) / (maxLon - minLon)) * 500;


            gr.DrawLine(myPen, normalizedp1Lat, normalizedp1Lon, normalizedp2Lat, normalizedp2Lon);
            //gr.DrawLine(myPen, 0, 0, 0, 0);

            //firstPoint = firstPoint

        }
    }
}
