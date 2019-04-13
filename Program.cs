using System;
using System.ServiceProcess;


namespace W1.CurrencyUpdater
{
    class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            CurrencyUpdater cu = new CurrencyUpdater();
            cu.fff();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CurrencyUpdater()
            };
            //ServiceBase.Run(ServicesToRun);
        }
    }
}