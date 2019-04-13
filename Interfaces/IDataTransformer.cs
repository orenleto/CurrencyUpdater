using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W1.CurrencyUpdater.Configuration;

namespace W1.CurrencyUpdater.Interfaces
{
    public interface IDataTransformer
    {
        [Export("DataTransformer")]
        IEnumerable<Valute> Transform(string src);
    }
}
