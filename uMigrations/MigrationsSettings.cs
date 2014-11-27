using System;
using System.Configuration;

namespace uMigrations
{
    public interface IMigrationSettings
    {
        int SystemUserId { get; }
        bool SkipMigrations { get; }
        bool EmulateMigrations { get; }
        string MigrationRuntimeId { get; }
    }

    public class MigrationsSettings : IMigrationSettings
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

        public virtual bool EmulateMigrations
        {
            get { return GetBooleanAppSetting("uMigrations.Emulate", false); }
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