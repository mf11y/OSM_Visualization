using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace OSM_Visualization
{
    class MapDrawer
    {
        Bitmap bitmap;
        OSMDataManager loader;
        DBPanel mainPanel;
        Graphics gr;
        ConcurrentBag<Tuple<float, float,float, float>> bag;


        public MapDrawer(DBPanel panel, ref OSMDataManager importedLoader, ref Bitmap bit)
        {
            loader = importedLoader;
            mainPanel = panel;
            bitmap = bit;
            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            bag = new ConcurrentBag<Tuple<float, float, float, float>>();
        }

        public void DrawMap()
        {
            gr.Clear(Color.White);
            GetPointsAndConnect();
        }

        private void GetPointsAndConnect()
        {

            var Nodes = loader.osmFile.Root.Elements("node");

            Parallel.ForEach(loader.waysConnectionInfo, way =>
            {
                float p1Lat = -1;
                float p1Lon = -1;
                float p2Lat = -1;
                float p2Lon = -1;
                float current = 0;


                foreach (var node in way)
                {
                    var specificNode = Nodes
                                        .First(z => z.Attribute("id").Value == node);

                    if (current == 0)
                    {
                       p1Lat = float.Parse(specificNode.Attribute("lat").Value);
                       p1Lon = float.Parse(specificNode.Attribute("lon").Value);
                       current++;
                    }
                    else
                    {
                       p2Lat = float.Parse(specificNode.Attribute("lat").Value);
                       p2Lon = float.Parse(specificNode.Attribute("lon").Value);

                       current--;
                    }

                    if (p1Lat != -1 && p2Lat != -1 && p1Lon != -1 && p2Lon != -1)
                    {
                        bag.Add(new Tuple<float, float, float, float>(p1Lat, p1Lon, p2Lat, p2Lon));
                    }
                }
            });

            ConnectPoints();
        }

        private static readonly Pen myPen = new Pen(Brushes.Red, 2);

        private void ConnectPoints()
        {
            float normalizedp1Lat;
            float normalizedp2Lon;
            float normalizedp1Lon;
            float normalizedp2Lat;

            float rotatedp1Lat;
            float rotatedp2Lat;

            foreach (var x in bag)
            {
                normalizedp1Lat = ((x.Item1 - loader.minLat) / (loader.maxLat - loader.minLat)) * 1000f;
                normalizedp1Lon = ((x.Item2 - loader.minLon) / (loader.maxLon - loader.minLon)) * 1000f;
                rotatedp1Lat = (normalizedp1Lat * -1) + 1000f;


                normalizedp2Lat = ((x.Item3 - loader.minLat) / (loader.maxLat - loader.minLat)) * 1000f;
                normalizedp2Lon = ((x.Item4 - loader.minLon) / (loader.maxLon - loader.minLon)) * 1000f;
                rotatedp2Lat = (normalizedp2Lat * -1) + 1000f;

                gr.DrawLine(myPen, normalizedp2Lon, rotatedp2Lat, normalizedp1Lon, rotatedp1Lat);

                mainPanel.Invalidate();
                //Thread.Sleep(1);
            }


            //bitmap.Save(@"c:\temp\bmap.bmp");

            /*if (counter % 1000 == 0)
            {
                mainPanel.Invalidate();
                counter = 0;
            }*/

            //mainPanel.Invalidate();
        }
    }

}
