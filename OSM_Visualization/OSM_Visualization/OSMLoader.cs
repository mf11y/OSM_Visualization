using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OSM_Visualization
{
    class OSMDataManager
    {
        public float minLat { get; private set; }
        public float minLon { get; private set; }
        public float maxLat { get; private set; }
        public float maxLon { get; private set; }

        public XDocument osmFile { get; private set; }

        public IEnumerable<IEnumerable<string>> waysConnectionInfo { get; private set; }

        public OSMDataManager(string fileLoc)
        {
            osmFile = XDocument.Load(fileLoc);

            ParseLatLonBounds();
            ParseNodeInfo();
            Thread.Sleep(200);

        }
        private void ParseLatLonBounds()
        {
            minLat = float.Parse(osmFile.Root.Element("bounds").Attribute("minlat").Value);
            minLon = float.Parse(osmFile.Root.Element("bounds").Attribute("minlon").Value);
            maxLat = float.Parse(osmFile.Root.Element("bounds").Attribute("maxlat").Value);
            maxLon = float.Parse(osmFile.Root.Element("bounds").Attribute("maxlon").Value);
        }

        private void ParseNodeInfo() => waysConnectionInfo = osmFile.Descendants("way")
                                        .Where(w => w.Elements("tag")
                                        .Any(a => (string)a.Attribute("k") == "building"))
                                        .Select(x => x.Elements("nd")
                                        .Select(z => z.Attribute("ref").Value)).ToList();

    }
}
