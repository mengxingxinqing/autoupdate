using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AutoUpdateHelper
{
    [XmlRoot("manifest")]
    public class Manifest
    {
        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("webpath")]
        public string WebPath { get; set; }

        [XmlElement("exepath")]
        public string ExePath { get; set; }
    }
}
