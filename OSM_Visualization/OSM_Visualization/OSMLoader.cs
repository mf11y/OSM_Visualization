using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;



namespace OSM_Visualization
{
    class OSMDataManager : IDisposable
    {

        //OSM file contains these data. Important for drawing map bounds
        public float minLat { get; private set; }
        public float minLon { get; private set; }
        public float maxLat { get; private set; }
        public float maxLon { get; private set; }

        //Dictionary for fast lookup of node info
        public Dictionary<string, Tuple<string,string>> dict;

        //Each internal list contains a list of nodes
        public List<List<string>> waysConnectionInfo { get; private set; }

        //Calls parseXML
        public OSMDataManager(string fileLoc)
        {

            XmlReader xReader = XmlReader.Create(fileLoc);


            int concurrencyLevel = Environment.ProcessorCount * 2;
            dict = new Dictionary<string, Tuple<string, string>>();
            waysConnectionInfo = new List<List<string>>();

            ParseXML(ref xReader);
            xReader.Dispose();

        }

        //Sequentially goes over the xml file and parses the data it needs to create a drawing of a map
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
                            dict.Add(File.GetAttribute("id"),
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
