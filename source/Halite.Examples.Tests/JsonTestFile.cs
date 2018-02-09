using System;
using System.IO;
using System.Reflection;

namespace Halite.Examples.Tests
{
    public static class JsonTestFile
    {
        public static string Read(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codeBaseUrl = new Uri(assembly.CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            var testDirPath = Path.Combine(dirPath, "TestFiles");
            var jsonFilePath = Path.Combine(testDirPath, fileName);
            return string.Join("\n", File.ReadAllLines(jsonFilePath));
        }
    }
}