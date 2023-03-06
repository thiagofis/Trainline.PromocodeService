using System;
using System.IO;
using System.Reflection;

namespace Trainline.PromocodeService.Host.Configuration
{
    public static class ConfigurationHelper
    {
        public static string GetFullPath(string relativeFileName)
        {
            return Path.Combine(GetCodeBaseFolder(), relativeFileName);
        }

        public static string GetFullPath(string relativeFolder, string fileName)
        {
            return Path.Combine(GetCodeBaseFolder(), relativeFolder, fileName);
        }

        private static string GetCodeBaseFolder()
        {
            var folderUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return Path.GetDirectoryName(folderUri.LocalPath);
        }
    }
}