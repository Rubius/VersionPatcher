using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        private string ReplaceToken(string value, int current)
        {
            return value == "*" ? current.ToString() : value;
        }

        private string ProcessVersionString(string currentValue, string newValuePattern, string attributeName)
        {
            var containsWildCard = currentValue.Contains("*");
            var versionPattern = "{0}.{1}.{2}.{3]";

            if (containsWildCard)
            {
                if (currentValue.Split('.').Length == 3)
                {
                    currentValue = currentValue.Replace("*", "0.0");
                    versionPattern = "{0}.{1}.*";
                }
                else
                {
                    currentValue = currentValue.Replace("*", "0");
                    versionPattern = "{0}.{1}.{2}.*";
                }
            }

            var version = new Version(currentValue);

            var tokens = newValuePattern.Split('.');
            if (tokens.Length < 2 || tokens.Length > 4)
            {
                throw new FormatException($"Specified value for {attributeName} has incorrect format.");
            }

            var versionBuilder = new StringBuilder(ReplaceToken(tokens[0], version.Major));
            versionBuilder.Append(".");
            versionBuilder.Append(ReplaceToken(tokens[1], version.Minor));
            if (tokens.Length > 2)
            {
                versionBuilder.Append(".");
                versionBuilder.Append(ReplaceToken(tokens[2], version.Build));

                if (tokens.Length > 3)
                {
                    versionBuilder.Append(".");
                    versionBuilder.Append(ReplaceToken(tokens[3], version.Revision));
                }
            }

            version = new Version(versionBuilder.ToString());

            return containsWildCard ? string.Format(versionPattern, version.Major, version.Minor, version.Build, version.Revision) 
                : version.ToString();
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
                        SyntaxFactory.Literal(
                            ProcessVersionString(currentValue, _assemblyVersion, "AssemblyVersion")
                            )));

                    break;

                case "AssemblyFileVersion":
                    if (_assemblyFileVersion == null)
                    {
                        break;
                    }

                    result = node.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(
                            ProcessVersionString(currentValue, _assemblyFileVersion, "AssemblyFileVersion")
                            )));

                    break;

                case "AssemblyInformationalVersion":
                    throw new NotImplementedException("Usages of this attribute is unknown...");
            }

            return result;
        }
    }
}
