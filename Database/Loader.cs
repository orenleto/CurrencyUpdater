using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W1.CurrencyUpdater.Configuration;

namespace W1.CurrencyUpdater.Database
{
    static class Loader
    {
        public static void LoadDataInDb(IEnumerable<Valute> valutes)
        {
            using (var connect = Connection.GetConnection(true))
            {
                using (var cmd = new SqlCommand("Exchange.UpdateBaseRates", connect))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var valuteCollection = new DataTable();

                    valuteCollection.Columns.Add("ExchangerId", typeof(string)).MaxLength = 16;
                    valuteCollection.Columns.Add("BaseRate", typeof(decimal));
                    valuteCollection.Columns.Add("Date", typeof(DateTime));

                    foreach (Valute valute in valutes)
                    {
                        valuteCollection.Rows.Add(valute.LocalName, valute.Rate, valute.Date);
                    }

                    cmd.Parameters.Add(new SqlParameter("Valutes", valuteCollection));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
