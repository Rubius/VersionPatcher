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
        [Alias("bn")]
        public int BuildNumber { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteCommandDetail($"Processing {File}...");

            var plist = new InfoPlist(File);
            var version = plist.Version;

            Version newVersion;
            newVersion = version.Build == -1 
                ? new Version(version.Major, version.Minor, BuildNumber) 
                : new Version(version.Major, version.Minor, version.Build, BuildNumber);

            plist.Version = newVersion;

            plist.Write();

            WriteCommandDetail("Processing complete");
        }
    }
}
