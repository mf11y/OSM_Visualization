using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            XDocument osmFile;

            osmFile = XDocument.Load(fileLoc);

            int concurrencyLevel = Environment.ProcessorCount * 2;
            dict = new ConcurrentDictionary<string, Tuple<float, float>>(concurrencyLevel, 2000003);

            waysConnectionInfo = new List<List<string>>();

            ParseLatLonBounds(ref osmFile);
            ParseWayInfo(ref osmFile);
            ParseNodeInfo(ref osmFile);

            osmFile = null;
            GC.Collect();

        }
        private void ParseLatLonBounds(ref XDocument File)
        {

            minLat = float.Parse(File.Root.Element("bounds").Attribute("minlat").Value);
            minLon = float.Parse(File.Root.Element("bounds").Attribute("minlon").Value);
            maxLat = float.Parse(File.Root.Element("bounds").Attribute("maxlat").Value);
            maxLon = float.Parse(File.Root.Element("bounds").Attribute("maxlon").Value);
        }

        private void ParseWayInfo(ref XDocument File)
        {
            var waysConnectionInfo1 = File.Root.Elements("way")
                                        .Where(w => w.Elements("tag")
                                        .Any(a => (string)a.Attribute("k") == "highway"))
                                        .Select(x => x.Elements("nd")
                                        .Select(z => z.Attribute("ref").Value).AsParallel()
                                        .ToList());

            waysConnectionInfo = waysConnectionInfo1.ToList();


        }

        private void ParseNodeInfo(ref XDocument File)
        {
            var Nodes = File.Root.Elements("node").ToList();

            /*int concurrencyLevel = Environment.ProcessorCount * 2;
            dict = new ConcurrentDictionary<string, Tuple<float, float>>(concurrencyLevel, 2000003);*/

            Parallel.ForEach(Nodes, node =>
            {
                dict.TryAdd(node.Attribute("id").Value,
                                new Tuple<float, float>(float.Parse(node.Attribute("lat").Value), float.Parse(node.Attribute("lon").Value))
                                );
            });
        }

    }
}
