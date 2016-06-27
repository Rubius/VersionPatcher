using System;
using System.Text;

namespace PropertiesEditor.Common
{
    internal static class VersionHelper
    {
        private static string ReplaceToken(string value, int current)
        {
            return value == "*" ? current.ToString() : value;
        }

        internal static string ProcessVersionString(string currentValue, string newValuePattern, string attributeName)
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

            return containsWildCard ? String.Format(versionPattern, version.Major, version.Minor, version.Build, version.Revision) 
                : version.ToString();
        }
    }
}
