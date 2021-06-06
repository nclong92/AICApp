using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AICListener.Code
{
    public class SettingsExtensions
    {
        public static string GetValue(string key)
        {
            return Properties.Settings.Default[key].ToString();
        }

        public static void SetValue(string value)
        {
            Properties.Settings.Default.AICServerName = value;
            Properties.Settings.Default.Save();
        }
    }
}
