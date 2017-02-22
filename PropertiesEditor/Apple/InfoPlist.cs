namespace PropertiesEditor.Apple
{
    using System;
    using System.IO;
    using System.Xml;

    public class InfoPlist
    {
        private readonly string _path;
        private readonly XmlDocument _doc;

        private XmlElement GetVersionNode()
        {
            var node = _doc.SelectSingleNode("/plist/dict/key[text()='CFBundleVersion']/following-sibling::string");
            return node as XmlElement;
        }

        public Version Version
        {
            get
            {
                return new Version(GetVersionNode().InnerText);
            }
            set { GetVersionNode().InnerText = value.ToString(); }
        }

        public void Write()
        {
            _doc.Save(new FileStream(_path, FileMode.Create));
            var xml = File.ReadAllText(_path);
            var correctedXml = xml.Replace(@"dtd""[]", @"dtd""");
            File.WriteAllText(_path, correctedXml);

        }
        public InfoPlist(string path)
        {
            _path = path;
            _doc = new XmlDocument();
            _doc.Load(new FileStream(path, FileMode.Open));
        }
    }
}
