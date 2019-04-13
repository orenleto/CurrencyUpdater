using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W1.CurrencyUpdater.Database
{
    public static class Connection
    {
        public static SqlConnection GetConnection()
        {
            return GetConnection(false);
        }

        public static SqlConnection GetConnection(bool openAfterCreate)
        {
            //throw new WalletServerException("Ведутся технологические работы. Сервис временно недоступен");
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Wallet.CurrencyUpdater.Properties.Settings.W1Connection"].ToString());
            if (openAfterCreate)
                connection.Open();
            return connection;
        }
    }
}
