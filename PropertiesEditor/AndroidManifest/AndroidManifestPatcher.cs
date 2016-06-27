using PropertiesEditor.Common;

namespace PropertiesEditor.AndroidManifest
{
    using System.Text.RegularExpressions;

    internal class AndroidManifestPatcher : ManifestPatcherBase<AndroidManifest>
    {
        public int VersionCode { get; set; }

        protected override void DoVersionChange(string versionPattern, AndroidManifest manifest)
        {
            base.DoVersionChange(versionPattern, manifest);

            manifest.VersionCode = VersionCode;
        }

        protected override string ManifestFileName => "AndroidManifest.xml";
    }

}
