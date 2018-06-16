using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OSM_Visualization
{
    class OSMDataManager
    {
        public float minLat { get; private set; }
        public float minLon { get; private set; }
        public float maxLat { get; private set; }
        public float maxLon { get; private set; }

        public ConcurrentDictionary<string, Tuple<float,float>> dict;


        public List<List<string>> waysConnectionInfo { get; private set; }
        //public IEnumerable<IEnumerable<string>> waysConnectionInfo;

        public OSMDataManager(string fileLoc)
        {
            //XDocument osmFile;

            //osmFile = XDocument.Load(fileLoc);

            XmlReader xReader = XmlReader.Create(fileLoc);
            //xReader.MoveToContent();

            int concurrencyLevel = Environment.ProcessorCount * 2;
            dict = new ConcurrentDictionary<string, Tuple<float, float>>(concurrencyLevel, 2000003);

            waysConnectionInfo = new List<List<string>>();

            ParseXML(ref xReader);
            Thread.Sleep(1);

            //osmFile = null;
            //GC.Collect();

        }
        private void ParseXML(ref XmlReader File)
        {
            while (File.Read())
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
                                        {
                                            if (inner.GetAttribute("k") == "highway")
                                            {
                                                valid = true;
                                            }

                                                break;
                                        }
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
                                            new Tuple<float, float>(float.Parse(File.GetAttribute("lat")), float.Parse(File.GetAttribute("lon"))));
                            break;
                        case "member":
                                break;
                    }
                }

            }
        }
    }
}
