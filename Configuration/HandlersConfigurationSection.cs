using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W1.CurrencyUpdater.Configuration
{
    class HandlerSection : ConfigurationSection
    {
        [ConfigurationProperty("HandlerCollection")]
        public HandlerCollection HandlerItems {
            get { return ((HandlerCollection)(base["HandlerCollection"])); }
        }

        public static HandlerSection Get()
        {
            return (HandlerSection)ConfigurationManager.GetSection("HandlerSection");
        }
    }

    [ConfigurationCollection(typeof(Handler), AddItemName = "Handler")]
    public class HandlerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Handler();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Handler)(element)).Name;
        }

        public Handler this[int idx] {
            get { return (Handler)BaseGet(idx); }
        }
    }

    public class Handler : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("used", DefaultValue = "false", IsRequired = true)]
        public bool Used {
            get { return (bool)this["used"]; }
            set { this["used"] = value; }
        }
    }
}
