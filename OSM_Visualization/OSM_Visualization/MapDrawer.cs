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


        public MapDrawer(DBPanel panel, ref Bitmap bit)
        {
            mainPanel = panel;
            bitmap = bit;
            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            bag = new ConcurrentBag<Tuple<float, float, float, float>>();
        }

        public void DrawMap(ref OSMDataManager xmlData)
        {
            loader = xmlData;
            gr.Clear(Color.DarkGreen);
            GetPoint();
            ConnectPoints();
            bag = null;
        }

        private void GetPoint()
        {
            Parallel.ForEach(loader.waysConnectionInfo, way =>
            {
                float p1Lat = 0;
                float p1Lon = 0;
                float p2Lat = 0;
                float p2Lon = 0;


                for(int i = 0; i < way.Count() - 1; i++)
                {
                    p1Lat = loader.dict[way[i]].Item1;
                    p1Lon = loader.dict[way[i]].Item2;
                    p2Lat = loader.dict[way[i + 1]].Item1;
                    p2Lon = loader.dict[way[i + 1]].Item2;
                    bag.Add(new Tuple<float, float, float, float>(p1Lat, p1Lon, p2Lat, p2Lon));
                }
            });
        }

        private static readonly Pen myPen = new Pen(Brushes.White, .1f);

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
                normalizedp1Lat = ((x.Item1 - loader.minLat) / (loader.maxLat - loader.minLat)) * (float )bitmap.Height;
                normalizedp1Lon = ((x.Item2 - loader.minLon) / (loader.maxLon - loader.minLon)) * (float )bitmap.Width;
                rotatedp1Lat = (normalizedp1Lat * -1) + (float)bitmap.Height;


                normalizedp2Lat = ((x.Item3 - loader.minLat) / (loader.maxLat - loader.minLat)) * (float)bitmap.Height;
                normalizedp2Lon = ((x.Item4 - loader.minLon) / (loader.maxLon - loader.minLon)) * (float)bitmap.Width;
                rotatedp2Lat = (normalizedp2Lat * -1) + (float)bitmap.Height;

                gr.DrawLine(myPen, normalizedp2Lon, rotatedp2Lat, normalizedp1Lon, rotatedp1Lat);

                //mainPanel.Invalidate();
               //Thread.Sleep(1);
            }


            //bitmap.Save(@"c:\temp\bmap.bmp");

            /*if (counter % 1000 == 0)
            {
                mainPanel.Invalidate();
                counter = 0;
            }*/

            mainPanel.Invalidate();
        }
    }

}
