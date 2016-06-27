using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PropertiesEditor.Common;

namespace PropertiesEditor.AssemblyInfo
{
    internal class AssemblyAttributeValueRewriter : CSharpSyntaxRewriter
    {
        private readonly string _assemblyVersion;
        private readonly string _assemblyFileVersion;
        private readonly string _assemblyInformationalVersion;

        public AssemblyAttributeValueRewriter(string assemblyVersion, string assemblyFileVersion, string assemblyInformationalVersion)
        {
            _assemblyInformationalVersion = assemblyInformationalVersion;
            _assemblyFileVersion = assemblyFileVersion;
            _assemblyVersion = assemblyVersion;
        }

        public override SyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            var attributeSyntax = (AttributeSyntax) node.Parent.Parent;

            SyntaxNode result = node;

            var currentValue = node.ToString().Trim('"');

            switch (attributeSyntax.Name.ToString())
            {
                case "AssemblyVersion":
                    if (_assemblyVersion == null)
                    {
                        break;
                    }

                    result = node.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
                        SyntaxFactory.Literal(VersionHelper.ProcessVersionString(currentValue, _assemblyVersion, "AssemblyVersion")
                            )));

                    break;

                case "AssemblyFileVersion":
                    if (_assemblyFileVersion == null)
                    {
                        break;
                    }

                    result = node.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(VersionHelper.ProcessVersionString(currentValue, _assemblyFileVersion, "AssemblyFileVersion")
                            )));

                    break;

                case "AssemblyInformationalVersion":
                    throw new NotImplementedException("Usages of this attribute is unknown...");
            }

            return result;
        }
    }
}
