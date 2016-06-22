using System.Collections.Generic;
using System.Management.Automation;

namespace PropertiesEditor.AssemblyInfo
{
    [Cmdlet(VerbsData.Edit, "AssemblyInfo")]
    [OutputType(typeof(PSPrimitiveDictionary))]
    public class EditAssemblyInfoCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipeline = true, Position = 0, Mandatory = true)]
        public string File { get; set; }

        [Parameter]
        [Alias("Version", "v")]
        public string AssemblyVersion { get; set; }

        [Parameter]
        [Alias("FileVersion", "fv")]
        public string AssemblyFileVersion { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {

            }
            finally
            {

            }
        }
    }
}
