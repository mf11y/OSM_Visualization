﻿using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;


namespace OSM_Visualization
{
    class MapDrawer : IDisposable
    {
        Bitmap bitmap;
        OSMDataManager loader;
        Graphics gr;

        //linePoints has all the lines needed to draw a map.
        ConcurrentBag<Tuple<float, float, float, float>> linePoints;

        //The points need to be transformed (rotated and normalized) to draw onto a bitmap.Contains tranformed points
        ConcurrentBag<Tuple<float, float, float, float>> transformedPoints;


        public MapDrawer(Tuple<int, int> bitmapInfo)
        {
            bitmap = new Bitmap(bitmapInfo.Item1, bitmapInfo.Item2);

            gr = Graphics.FromImage(bitmap);
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            linePoints = new ConcurrentBag<Tuple<float, float, float, float>>();
            transformedPoints = new ConcurrentBag<Tuple<float, float, float, float>>();
        }

        //Gets the lines needed to draw map, tranforms, and draws onto a bitmap
        public Bitmap DrawMap(ref OSMDataManager xmlData)
        {
            loader = xmlData;

            GetPoints();
            TransformPoints();
            DrawToBitmap();

            transformedPoints = new ConcurrentBag<Tuple<float, float, float, float>>();

            return bitmap;
        }

        //Loops through the list of node lists to save end points of a line
        private void GetPoints()
        {
            Parallel.ForEach(loader.waysConnectionInfo, way =>
            {
                float p1Lat = 0;
                float p1Lon = 0;
                float p2Lat = 0;
                float p2Lon = 0;


                for (int i = 0; i < way.Count() - 1; i++)
                {
                    p1Lat = float.Parse(loader.dict[way[i]].Item1);
                    p1Lon = float.Parse(loader.dict[way[i]].Item2);
                    p2Lat = float.Parse(loader.dict[way[i + 1]].Item1);
                    p2Lon = float.Parse(loader.dict[way[i + 1]].Item2);
                    linePoints.Add(new Tuple<float, float, float, float>(p1Lat, p1Lon, p2Lat, p2Lon));
                }
            });
        }

        //TRansform the points concurrently. normalize and rotate
        private void TransformPoints()
        {
            int H = bitmap.Height;
            int W = bitmap.Width;

            Parallel.ForEach(linePoints, x =>
            {

                transformedPoints.Add(NormalizeAndRotate(x, H, W));

            });
        }

        private Tuple<float, float, float, float> NormalizeAndRotate(Tuple<float, float, float, float> unNormalizedValues, int H, int W)
        {
            float normalizedp1Lat;
            float normalizedp2Lon;
            float normalizedp1Lon;
            float normalizedp2Lat;

            float rotatedp1Lat;
            float rotatedp2Lat;

            normalizedp1Lat = ((unNormalizedValues.Item1 - loader.minLat) / (loader.maxLat - loader.minLat)) * H;
            normalizedp1Lon = ((unNormalizedValues.Item2 - loader.minLon) / (loader.maxLon - loader.minLon)) * W;
            rotatedp1Lat = (normalizedp1Lat * -1) + H;


            normalizedp2Lat = ((unNormalizedValues.Item3 - loader.minLat) / (loader.maxLat - loader.minLat)) * H;
            normalizedp2Lon = ((unNormalizedValues.Item4 - loader.minLon) / (loader.maxLon - loader.minLon)) * W;
            rotatedp2Lat = (normalizedp2Lat * -1) + H;

            return (new Tuple<float, float, float, float>(normalizedp2Lon, rotatedp2Lat, normalizedp1Lon, rotatedp1Lat));
        }

        private static readonly Pen myPen = new Pen(Brushes.White, 2);

        //Draws the transformed points to bitmap
        private void DrawToBitmap()
        {
            gr.Clear(Color.Black);

            foreach (var x in transformedPoints)
            {
                gr.DrawLine(myPen, x.Item1, x.Item2, x.Item3, x.Item4);
            }
        }


        public void Dispose()
        {
            linePoints = null;
            transformedPoints = null;
            loader = null;
            bitmap.Dispose();
            gr.Dispose();
            myPen.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
