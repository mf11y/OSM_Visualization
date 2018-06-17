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
    class MapDrawer : IDisposable
    {
        Bitmap bitmap;
        OSMDataManager loader;
        DBPanel mainPanel;
        Graphics gr;
        ConcurrentBag<Tuple<float, float,float, float>> bag;
        ConcurrentBag<Tuple<float, float, float, float>> Transformed;


        public MapDrawer(DBPanel panel, ref Bitmap bit)
        {
            mainPanel = panel;
            bitmap = bit;
            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            bag = new ConcurrentBag<Tuple<float, float, float, float>>();
            Transformed = new ConcurrentBag<Tuple<float, float, float, float>>();
        }

        public void DrawMap(ref OSMDataManager xmlData, float zoomFactor)
        {
            loader = xmlData;

            if(zoomFactor > .9f)
                GetPoints();

            ConnectPoints(zoomFactor);
        }

        private void GetPoints()
        {
            Parallel.ForEach(loader.waysConnectionInfo, way =>
            {
                float p1Lat = 0;
                float p1Lon = 0;
                float p2Lat = 0;
                float p2Lon = 0;


                for(int i = 0; i < way.Count() - 1; i++)
                {
                    p1Lat = float.Parse(loader.dict[way[i]].Item1);
                    p1Lon =float.Parse(loader.dict[way[i]].Item2);
                    p2Lat = float.Parse(loader.dict[way[i + 1]].Item1);
                    p2Lon = float.Parse(loader.dict[way[i + 1]].Item2);
                    bag.Add(new Tuple<float, float, float, float>(p1Lat, p1Lon, p2Lat, p2Lon));
                }
            });
        }

        private static readonly Pen myPen = new Pen(Brushes.White, .1f);

        private void ConnectPoints(float zoomFactor)
        {
            float diff1 = loader.maxLon - loader.minLon;
            float diff2 = loader.maxLat - loader.minLat;

            if (zoomFactor < 1f)
            {
                gr.Clear(Color.Gray);
                mainPanel.Invalidate();
                float zoomed1 = diff2 * zoomFactor;
                float zoomed2 = diff1 * zoomFactor;

                float sub1 = (diff2 - zoomed1) / 2f ;
                loader.maxLat -= sub1;
                loader.minLat += sub1;

                float sub2 = (diff1 - zoomed2) /2f;
                loader.maxLon -= sub2;
                loader.minLon += sub2;
            }


            float H = (float)bitmap.Height;
            float W = (float)bitmap.Width;

            Parallel.ForEach(bag, x =>
            {
                float normalizedp1Lat;
                float normalizedp2Lon;
                float normalizedp1Lon;
                float normalizedp2Lat;

                float rotatedp1Lat;
                float rotatedp2Lat;

                if (x.Item1 < loader.maxLat && x.Item1 > loader.minLat &&
                   x.Item2 < loader.maxLon && x.Item2 > loader.minLon &&
                   x.Item3 < loader.maxLat && x.Item3 > loader.minLat &&
                   x.Item2 < loader.maxLon && x.Item2 > loader.minLon)
                {

                    normalizedp1Lat = ((x.Item1 - loader.minLat) / (loader.maxLat - loader.minLat)) * H;
                    normalizedp1Lon = ((x.Item2 - loader.minLon) / (loader.maxLon - loader.minLon)) * W;
                    rotatedp1Lat = (normalizedp1Lat * -1) + H ;


                    normalizedp2Lat = ((x.Item3 - loader.minLat) / (loader.maxLat - loader.minLat)) * H;
                    normalizedp2Lon = ((x.Item4 - loader.minLon) / (loader.maxLon - loader.minLon)) * W;
                    rotatedp2Lat = (normalizedp2Lat * -1) + H;

                    Transformed.Add(new Tuple<float, float, float, float>(normalizedp2Lon, rotatedp2Lat, normalizedp1Lon, rotatedp1Lat));
                }
            });

            gr.Clear(Color.Gray);

            foreach (var x in Transformed)
            {
                gr.DrawLine(myPen, x.Item1, x.Item2, x.Item3, x.Item4);
                //mainPanel.Invalidate();
            }

            Transformed = new ConcurrentBag<Tuple<float, float, float, float>>();

            mainPanel.Invalidate();

        }

        public void Dispose()
        {
            bag = null;
            Transformed = null;
            bitmap = null;
            gr = null;
            loader = null;
            GC.SuppressFinalize(this);
        }
    }

}
