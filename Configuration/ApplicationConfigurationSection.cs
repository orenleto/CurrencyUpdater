using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W1.CurrencyUpdater.Configuration
{
    class ApplicationSection : ConfigurationSection
    {
        [ConfigurationProperty("time", DefaultValue = 20, IsRequired = true)]
        public int Timer {
            get { return ((int)(base["time"])); }
            set { base["time"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path {
            get { return ((string)(base["path"])); }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("email", IsRequired = false)]
        public string Email {
            get { return ((string)(base["email"])); }
            set { base["email"] = value; }
        }

        public static ApplicationSection Get()
        {
            return (ApplicationSection)ConfigurationManager.GetSection("ApplicationSection");
        }
    }
}
