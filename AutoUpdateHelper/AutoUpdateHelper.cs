using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AutoUpdateHelper
{
    public class AutoUpdate
    {
        public static bool CheckAndUpdate()
        {
            try
            {
                LocalConf local = new LocalConf();
                Uri uri = new Uri(local.Manifest);
                string doc = GetManifest(uri);
                XmlSerializer xser = new XmlSerializer(typeof(Manifest));
                var manifest = xser.Deserialize(new XmlTextReader(doc, XmlNodeType.Document, null)) as Manifest;
                if (manifest.Version != local.Version)
                {
                    Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, local.Update));
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static string GetVersion()
        {
            LocalConf local = new LocalConf();
            return local.Version;
        }

       

        private static string GetManifest(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;
            string response = String.Empty;
            using (WebResponse res = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), true))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }
    }
}
