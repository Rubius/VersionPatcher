using System;
using System.Management.Automation;
using Microsoft.CodeAnalysis.CSharp;

namespace PropertiesEditor.AssemblyInfo
{
    [Cmdlet(VerbsData.Edit, "AssemblyInfo")]
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

            WriteCommandDetail($"Processing file {File}...");

            try
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(System.IO.File.ReadAllText(File));
                var rewriter = new AssemblyAttributeValueRewriter(AssemblyVersion, AssemblyFileVersion, null);
                var newSyntaxTree = rewriter.Visit(syntaxTree.GetRoot());

                System.IO.File.WriteAllText(File, newSyntaxTree.ToString());
            }
            catch (Exception e)
            {
                WriteWarning($"{File} has not beed patched because {e}");
            }

            WriteCommandDetail($"Processing complete.");
        }
    }
}
