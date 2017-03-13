using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PropertiesEditor.Apple
{
    [Cmdlet(VerbsData.Edit, "InfoPlist")]
    public class EditInfoPlistCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipeline = true, Position = 0, Mandatory = true)]
        public string File { get; set; }

        [Parameter]
        [Alias("v")]
        public string Version { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                WriteCommandDetail($"Processing {File}...");

                var plist = new InfoPlist(File);

                var versionParam = new Version(Version);

                plist.Version = versionParam;

                plist.Write();
            }
            catch (Exception e)
            {
                WriteWarning($"{File} has not beed patched because {e}");
            }

            WriteCommandDetail("Processing complete");
        }
    }
}
