using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W1.CurrencyUpdater.Configuration;

namespace W1.CurrencyUpdater
{
    class ExchangerStorage
    {
        public Dictionary<Exchanger, DateTime> Storage { get; private set; }

        public ExchangerStorage()
        {
            Storage = new Dictionary<Exchanger, DateTime>();
        }

        public void Update(IEnumerable<Exchanger> exchangerList)
        {
            foreach (var item in Storage.Select(x => x.Key))
            {
                var record = exchangerList.FirstOrDefault(x => x.Name == item.Name);
                if (record == null)
                {
                    Application.Logger.Info(String.Format("{0} will be deleted from Storage", record.Name));
                    Storage.Remove(item);
                }
            }

            foreach (var item in exchangerList)
            {
                if (!Storage.ContainsKey(item))
                {
                    Application.Logger.Info(String.Format("{0} inserted in Storage", item.Name));
                    Storage.Add(item, DateTime.MinValue);
                }
            }
        }
        public void Update(Exchanger exchanger)
        {
            Storage[exchanger] = DateTime.Now;
        }
        public void Update()
        {
            foreach (var item in Storage.Select(x => x.Key))
            {
                Storage[item] = DateTime.MinValue;
            }
        }

        public override String ToString()
        {
            StringBuilder message = new StringBuilder();
            foreach (var item in Storage)
            {
                if (item.Value != DateTime.MinValue)
                {
                    message.AppendLine(String.Format("Источник {0} обновился в {1}", item.Key.Name, item.Value));
                }
                else
                {
                    message.AppendLine(String.Format("Источник {0} не обновился", item.Key.Name, item.Value));
                }
            }
            return message.ToString();
        }
    }
}
