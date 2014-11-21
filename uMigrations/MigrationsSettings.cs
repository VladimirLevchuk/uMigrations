using System;
using System.Configuration;

namespace uMigrations
{
    public class MigrationsSettings
    {
        private readonly Lazy<string> _migrationRuntimeId = new Lazy<string>(() => DateTime.Now.ToString("s").Replace("-", "_").Replace(":", "_"));

        public virtual int SystemUserId
        {
            get { return GetIntAppSetting("uMigrations.SystemUserId", 0); }
        }

        public virtual bool SkipMigrations
        {
            get { return GetBooleanAppSetting("uMigrations.Skip", false); }
        }

        public virtual string MigrationRuntimeId
        {
            get { return _migrationRuntimeId.Value; }
        }

        protected bool GetBooleanAppSetting(string name, bool defaultValue = false)
        {
            var stringValue = ConfigurationManager.AppSettings[name];
            if (stringValue == null)
            {
                return defaultValue;
            } 

            bool result;
            return bool.TryParse(stringValue, out result) ? result : defaultValue;
        }

        protected int GetIntAppSetting(string name, int defaultValue = 0)
        {
            var stringValue = ConfigurationManager.AppSettings[name];
            if (stringValue == null)
            {
                return defaultValue;
            }
            int result;
            return int.TryParse(stringValue, out result) ? result : defaultValue;
        }
    }
}