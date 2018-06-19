using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;



namespace OSM_Visualization
{
    class OSMDataManager : IDisposable
    {
        public float minLat { get; private set; }
        public float minLon { get; private set; }
        public float maxLat { get; private set; }
        public float maxLon { get; private set; }

        public ConcurrentDictionary<string, Tuple<string,string>> dict;
        public List<List<string>> waysConnectionInfo { get; private set; }

        public OSMDataManager(string fileLoc)
        {

            XmlReader xReader = XmlReader.Create(fileLoc);


            int concurrencyLevel = Environment.ProcessorCount * 2;
            dict = new ConcurrentDictionary<string, Tuple<string, string>>(concurrencyLevel, 2000003);
            waysConnectionInfo = new List<List<string>>();

            ParseXML(ref xReader);
            xReader.Dispose();

        }



        private void ParseXML(ref XmlReader File)
        {
            bool member = false;
            while (File.Read() && !member)
            {
                if (File.NodeType == XmlNodeType.Element)
                {
                    switch (File.Name)
                    {
                        case "bounds":
                            minLat = float.Parse(File.GetAttribute("minlat"));
                            minLon = float.Parse(File.GetAttribute("minlon"));
                            maxLat = float.Parse(File.GetAttribute("maxlat"));
                            maxLon = float.Parse(File.GetAttribute("maxlon"));
                            break;
                        case "way":
                            XmlReader inner = File.ReadSubtree();
                            List<string> temp = new List<string>();
                            bool valid = false;

                            while (inner.Read())
                            {
                                if(File.NodeType == XmlNodeType.Element)
                                {
                                    switch(inner.Name)
                                    {
                                        case "nd":
                                                temp.Add(inner.GetAttribute("ref"));
                                            break;
                                        case "tag":
                                            if (inner.GetAttribute("k") == "highway")
                                            {
                                                valid = true;
                                            }
                                            break;
                                    }
                                }
                            }
                            if (valid)
                            {
                                waysConnectionInfo.Add(temp);
                            }
                            break;
                        case "node":
                            dict.TryAdd(File.GetAttribute("id"),
                                            new Tuple<string, string>(File.GetAttribute("lat"), File.GetAttribute("lon")));
                            break;
                        case "member":
                            member = true;
                                break;
                    }
                }
            }
        }

        public void Dispose()
        {
            dict = null;
            waysConnectionInfo = null;
            GC.SuppressFinalize(this);
        }

    }
}
