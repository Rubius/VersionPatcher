namespace PropertiesEditor.Common
{
    using System;
    using System.IO;
    using System.Xml;

    public abstract class ManifestBase
    {
        protected XmlDocument _doc;
        private XmlNode _versionNode;

        public Version Version
        {
            get { return new Version(_versionNode.Value); }
            set { _versionNode.Value = value.ToString(); }
        }

        public void Load(string path)
        {
            _doc = new XmlDocument();
            _doc.LoadXml(File.ReadAllText(path));
            _versionNode = GetVersion();
        }

        protected abstract XmlNode GetVersion();

        public void Save(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                _doc.Save(fileStream);
            }
        }
    }
}
