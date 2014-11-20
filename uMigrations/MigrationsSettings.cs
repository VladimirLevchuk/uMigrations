using System.Configuration;

namespace uMigrations
{
    public class MigrationsSettings
    {
        public virtual int SystemUserId
        {
            get { return GetIntAppSetting("uMigrations.SystemUserId", 0); }
        }

        public virtual bool SkipMigrations
        {
            get { return GetBooleanAppSetting("uMigrations.Skip", false); }
        }

        protected bool GetBooleanAppSetting(string name, bool defaultValue = false)
        {
            var stringValue = ConfigurationManager.AppSettings[name];
            if (stringValue == null)
            {
                return defaultValue;
            } 

            var result = defaultValue;
            return bool.TryParse(stringValue, out result) ? result : defaultValue;
        }

        protected int GetIntAppSetting(string name, int defaultValue = 0)
        {
            var stringValue = ConfigurationManager.AppSettings[name];
            if (stringValue == null)
            {
                return defaultValue;
            }
            var result = defaultValue;
            return int.TryParse(stringValue, out result) ? result : defaultValue;
        }
    }
}