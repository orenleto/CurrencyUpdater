using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W1.CurrencyUpdater.Configuration;

namespace W1.CurrencyUpdater.Database
{
    public class DataKeeper
    {
        private Object thisLock = new Object();
        private List<Valute> Valutes { get; set; }
        public List<Valute> UpdateList { get; set; }

        public DataKeeper()
        {
            Valutes = new List<Valute>();
            UpdateList = new List<Valute>();
        }

        public int Update(IEnumerable<Valute> data, IEnumerable<Valute> valutes)
        {
            int count = 0;
            lock (thisLock)
            {
                foreach (Valute item in valutes)
                {
                    var record = data.FirstOrDefault(x => (x.DestinationValute == item.SourceValute && x.SourceValute == item.DestinationValute) ||
                                                          (x.DestinationValute == item.DestinationValute && x.SourceValute == item.SourceValute));

                    if (record != null)
                    {
                        item.Date = record.Date;
                        item.Rate = (record.DestinationValute == item.DestinationValute && record.SourceValute == item.SourceValute) ? record.Rate : record.ReverseRate;
                    }
                    else
                    {
                        Application.Logger.Error(String.Format("Not found info for {0}", item.ToString()));
                        continue;
                    }

                    record = Valutes.FirstOrDefault(x => x.LocalName == item.LocalName);
                    if (record != null)
                    {
                        if (record.CompareTo(item) == 0)
                            continue;
                        Valutes.Remove(record);
                    }
                    Valutes.Add(item);
                    UpdateList.Add(item);
                    ++count;
                }
            }
            return count;
        }
    }
}
