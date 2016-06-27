using System;
using System.IO;

namespace PropertiesEditor.Common
{
    public abstract class ManifestPatcherBase<T>  where T : ManifestBase, new()
    {
        public void Patch(string fileName, string versionPattern)
        {
            var manifest = new T();
            manifest.Load(fileName);

            DoVersionChange(versionPattern, manifest);

            manifest.Save(fileName);
        }

        protected virtual void DoVersionChange(string versionPattern, T manifest)
        {
            var version = manifest.Version;
            var newVersion = new Version(VersionHelper.ProcessVersionString(version.ToString(), versionPattern, "Manifest Version"));
            manifest.Version = newVersion;
        }

        protected abstract string ManifestFileName { get; }
    }
}
