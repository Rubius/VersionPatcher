namespace PropertiesEditor.AndroidManifest
{
    using System.Xml;
    using Common;

    public class AndroidManifest : ManifestBase
    {
        private XmlAttribute _versionCode;

        public int VersionCode
        {
            get { return int.Parse(_versionCode.Value); }
            set { _versionCode.Value = value.ToString(); }
        }

        protected override XmlNode GetVersion()
        {
            var namespaceManager = new XmlNamespaceManager(_doc.NameTable);
            namespaceManager.AddNamespace("a", @"http://schemas.android.com/apk/res/android");
            _versionCode = (XmlAttribute)_doc.SelectSingleNode(@"/manifest/@a:versionCode", namespaceManager);

            return _doc.SelectSingleNode(@"/manifest/@a:versionName", namespaceManager);
        }
    }
}
