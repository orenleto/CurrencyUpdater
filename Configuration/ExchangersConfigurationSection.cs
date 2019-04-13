using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;

namespace W1.CurrencyUpdater.Configuration
{
    public class ExchangerSection : ConfigurationSection
    {
        [ConfigurationProperty("ExchangerCollection")]
        public ExchangerCollection Exchangers {
            get { return this["ExchangerCollection"] as ExchangerCollection; }
        }

        public static ExchangerSection Get()
        {
            return (ExchangerSection)ConfigurationManager.GetSection("ExchangerSection");
        }
    }
    public class ExchangerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Exchanger();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Exchanger)element).Name;
        }
        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName {
            get { return "Exchanger"; }
        }
        public Exchanger this[int index] {
            get { return (Exchanger)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }
    public class Exchanger : ConfigurationElement, IComparable<Exchanger>
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Name {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("url", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Url {
            get { return ((string)(base["url"])); }
            set { base["url"] = value; }
        }

        [ConfigurationProperty("method", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Method {
            get { return ((string)(base["method"])); }
            set { base["method"] = value; }
        }

        [ConfigurationProperty("contenttype", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ContentType {
            get { return ((string)(base["contenttype"])); }
            set { base["contenttype"] = value; }
        }

        [ConfigurationProperty("body", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Body {
            get { return ((string)(base["body"])); }
            set { base["body"] = value; }
        }

        [ConfigurationProperty("handler", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string HandlerName {
            get { return ((string)(base["handler"])); }
            set { base["handler"] = value; }
        }

        [ConfigurationProperty("CurrencyCollection")]
        public CurrencyCollection Currencies {
            get { return this["CurrencyCollection"] as CurrencyCollection; }
        }

        public int CompareTo(Exchanger other)
        {
            return Name.CompareTo(other.Name);
        }
    }
    public class CurrencyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Valute();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Valute)element).LocalName;
        }
        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName {
            get { return "Currency"; }
        }
        public Valute this[int index] {
            get { return (Valute)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
        new public Valute this[string employeeID] {
            get { return (Valute)BaseGet(employeeID); }
        }
        public bool ContainsKey(string key)
        {
            bool result = false;
            object[] keys = BaseGetAllKeys();
            foreach (object obj in keys)
            {
                if ((string)obj == key)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
    public class Valute : ConfigurationElement, IComparable<Valute>
    {
        [ConfigurationProperty("value", IsRequired = true, IsKey = true)]
        public string LocalName {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }
        [ConfigurationProperty("src", IsRequired = true, IsKey = true)]
        public int SourceValute {
            get { return (int)this["src"]; }
            set { this["src"] = value; }
        }
        [ConfigurationProperty("dst", IsRequired = true, IsKey = true)]
        public int DestinationValute {
            get { return (int)this["dst"]; }
            set { this["dst"] = value; }
        }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
        public decimal ReverseRate { get; set; }

        public override String ToString()
        {
            return LocalName;
        }

        public int CompareTo(Valute item)
        {
            return (Date.CompareTo(item.Date) != 0 ||
                    Rate.CompareTo(item.Rate) != 0 ||
                    ReverseRate.CompareTo(item.ReverseRate) != 0) 
                   ? 1 : 0;
        }
    }
}
