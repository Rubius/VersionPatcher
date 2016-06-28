using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PropertiesEditor.AndroidManifest
{
    [Cmdlet(VerbsData.Edit, "AndroidManifest")]
    public class EditAndroidManifestCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipeline = true, Position = 0, Mandatory = true)]
        public string File { get; set; }

        [Parameter]
        [Alias("v")]
        public string Version { get; set; }

        [Parameter]
        [Alias("vc")]
        public int VersionCode { get; set; } = 0;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteCommandDetail($"Processing {File}...");

            if (string.IsNullOrWhiteSpace(Version))
            {
                return;
            }

            var patcher = new AndroidManifestPatcher()
            {
                VersionCode = VersionCode
            };

            patcher.Patch(File, Version);

            WriteVerbose($"Processing complete.");
        }
    }
}
