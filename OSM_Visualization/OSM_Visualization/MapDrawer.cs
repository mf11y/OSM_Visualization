using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM_Visualization
{
    class MapDrawer
    {
        Bitmap bitmap;
        OSMDataManager loader;
        DBPanel mainPanel;
        Graphics gr;

        public MapDrawer(DBPanel panel, ref OSMDataManager importedLoader, ref Bitmap bit)
        {
            loader = importedLoader;
            mainPanel = panel;
            bitmap = bit;
            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.AntiAlias;
        }

        public void DrawMap()
        {
            GetPointsAndConnect();
        }

        private void GetPointsAndConnect()
        {
            float p1Lat;
            float p1Lon;
            float p2Lat;
            float p2Lon;
            float current;

            foreach (var way in loader.waysConnectionInfo)
            {
                p1Lat = -1;
                p1Lon = -1;
                p2Lat = -1;
                p2Lon = -1;
                current = 0;

                foreach (var node in way.ToList())
                {
                    if (current == 0)
                    {
                        p1Lat = float.Parse(loader.osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == node).Attribute("lat").Value);
                        p1Lon = float.Parse(loader.osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == node).Attribute("lon").Value);
                        current++;
                    }
                    else
                    {
                        p2Lat = float.Parse(loader.osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == node).Attribute("lat").Value);
                        p2Lon = float.Parse(loader.osmFile.Root.Elements("node").First(z => z.Attribute("id").Value == node).Attribute("lon").Value);
                        current--;
                    }

                    if (p1Lat != -1 && p1Lon != -1 && p2Lat != -1 && p2Lon != -1)
                        ConnectPoints(p1Lat, p1Lon, p2Lat, p2Lon);
                }
            }
        }

        private static readonly Pen myPen = new Pen(Brushes.Red, 3);

        private void ConnectPoints(float p1Lat, float p1Lon, float p2Lat, float p2Lon)
        {
            float normalizedp1Lat = ((p1Lat - loader.minLat) / (loader.maxLat - loader.minLat)) * 1000f;
            float normalizedp1Lon = ((p1Lon - loader.minLon) / (loader.maxLon - loader.minLon)) * 1000f;
            normalizedp1Lat = (normalizedp1Lat * -1) + 1000f;

            float normalizedp2Lat = ((p2Lat - loader.minLat) / (loader.maxLat - loader.minLat)) * 1000f;
            float normalizedp2Lon = ((p2Lon - loader.minLon) / (loader.maxLon - loader.minLon)) * 1000f;
            normalizedp2Lat = (normalizedp2Lat * -1) + 1000f;

            gr.DrawLine(myPen, normalizedp2Lon, normalizedp2Lat, normalizedp1Lon, normalizedp1Lat);
            //bitmap.Save(@"c:\temp\bmap.bmp");

            mainPanel.Invalidate();
        }
    }

}
