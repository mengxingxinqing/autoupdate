using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AutoUpdateHelper
{
    class LocalConf
    {
        private static XmlDocument document = new XmlDocument();
        private static readonly string xmlFileName = Path.Combine(Environment.CurrentDirectory, "update.xml");

        static LocalConf()
        {
            document.Load(xmlFileName);
        }
        public string Version
        {
            get
            {
                return document.SelectSingleNode("localconf").SelectSingleNode("version").InnerText;
            }
            set
            {
                document.SelectSingleNode("localconf").SelectSingleNode("version").InnerText = value;
                document.Save(xmlFileName);
            }
        }


        public string Manifest
        {
            get
            {
                return document.SelectSingleNode("localconf").SelectSingleNode("manifest").InnerText;
                
            }
            set
            {
                document.SelectSingleNode("localconf").SelectSingleNode("manifest").InnerText = value;
                document.Save(xmlFileName);
            }
        }

        public string Update
        {
            get
            {
                return document.SelectSingleNode("localconf").SelectSingleNode("update").InnerText;

            }
            set
            {
                document.SelectSingleNode("localconf").SelectSingleNode("update").InnerText = value;
                document.Save(xmlFileName);
            }
        }
    }
}
