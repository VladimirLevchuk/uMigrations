using System;
using System.IO;
using System.Linq;
using Umbraco.Core;

namespace uMigrations.Tests
{
    public class TestApplication : UmbracoApplicationBase
    {
        public string BaseDirectory { get; private set; }
        public string DataDirectory { get; private set; }

        protected override IBootManager GetBootManager()
        {
            var binDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            BaseDirectory = ResolveBasePath(binDirectory);
            DataDirectory = Path.Combine(BaseDirectory, "app_data");
            
            var appDomainConfigPath = new DirectoryInfo(Path.Combine(binDirectory.FullName, "config"));

            //Copy config files to AppDomain's base directory
            if (binDirectory.FullName.Equals(BaseDirectory) == false)
            {
                RemoveDir(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_data"));

                if (appDomainConfigPath.Exists == false)
                {
                    appDomainConfigPath.Create();
                    var baseConfigPath = new DirectoryInfo(Path.Combine(BaseDirectory, "config"));
                    var sourceFiles = baseConfigPath.GetFiles("*.config", SearchOption.TopDirectoryOnly);
                    foreach (var sourceFile in sourceFiles)
                    {
                        sourceFile.CopyTo(
                            sourceFile.FullName.Replace(baseConfigPath.FullName, appDomainConfigPath.FullName), true);
                    }
                }
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", DataDirectory);

            return new TestBootManager(this, BaseDirectory);
        }

        private void RemoveDir(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return;
            }

            var files = Directory.EnumerateFiles(path);
            var directories = Directory.EnumerateDirectories(path);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var directory in directories)
            {
                RemoveDir(directory);
            }

            Directory.Delete(path);
        }
        
        public void Start()
        {
            if (string.IsNullOrEmpty(BaseDirectory))
            {
                var bootManager = GetBootManager();
            }

            base.Application_Start(this, new EventArgs());
        }

        private void _RemoveCopies(string dataDirectoryPath, string configDirectoryPath)
        {
            RemoveDir(configDirectoryPath);

            RemoveDir(dataDirectoryPath);
        }

        private void _RemoveDirectories()
        {
            var directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).ToList();

            directories.ForEach(RemoveDir);
        }

        public void Finish()
        {
            // var config = Path.Combine(BaseDirectory, "config");
            _RemoveDirectories();
        }

        private string ResolveBasePath(DirectoryInfo currentFolder)
        {
            var folders = currentFolder.GetDirectories();
            if (folders.Any(x => x.Name.Equals("app_data", StringComparison.OrdinalIgnoreCase)) &&
                folders.Any(x => x.Name.Equals("config", StringComparison.OrdinalIgnoreCase)))
            {
                return currentFolder.FullName;
            }

            if (currentFolder.Parent == null)
                throw new Exception("Base directory containing an 'App_Data' and 'Config' folder was not found." +
                                    " These folders are required to run this console application as it relies on the normal umbraco configuration files.");

            return ResolveBasePath(currentFolder.Parent);
        }
    }
}