using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using W1.CurrencyUpdater.Configuration;
using W1.CurrencyUpdater.Database;
using System.Text.RegularExpressions;
using W1.CurrencyUpdater.Network;

namespace W1.CurrencyUpdater
{
    public delegate IEnumerable<Valute> DataTransformer(string src);

    class Application
    {
        public static NLog.Logger Logger;
        public static Database.DataKeeper Keeper;
        public static ExchangerStorage Storage;

        public TimeSpan RestartInterval { get; private set; }
        public Uri ExtensionPath { get; private set; }
        public String[] Emails { get; set; }

        [ImportMany("DataTransformer")]
        public IEnumerable<DataTransformer> Transformers { get; set; }
        public IEnumerable<Exchanger> Exchangers { get; set; }

        public Application()
        {
            Logger = NLog.LogManager.GetCurrentClassLogger();
            Keeper = new Database.DataKeeper();
            Storage = new ExchangerStorage();
        }

        public bool Initialize()
        {
            if (!ApplicationInit())
            {
                EmailSender.Send(Emails, String.Format("Extension path not found on {0}", ExtensionPath));
                return false;
            }

            if (!HandlerInit())
            {
                EmailSender.Send(Emails, "Not all source has handler");
            }

            ExchangerInit();
            return true;
        }

        private bool ApplicationInit()
        {
            Logger.Trace("=== Application loader start ===");
            var config = ApplicationSection.Get();

            RestartInterval = new TimeSpan(config.Timer / 60, config.Timer % 60, 0);
            Logger.Debug("Restart interval is set on {0} minutes", RestartInterval.TotalMinutes);

            Emails = config.Email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (Directory.Exists(config.Path))
            {
                ExtensionPath = new Uri(config.Path);
                Logger.Debug("Extension path found on {0}", ExtensionPath.AbsolutePath);
            }
            else
            {
                ExtensionPath = new Uri(Directory.GetCurrentDirectory());
                Logger.Error("Extension path not found on {0}", config.Path);
                return false;
            }
            Logger.Debug("=== Application loader end ===");
            return true;
        }
        private bool HandlerInit()
        {
            Logger.Debug("=== Handler loader start ===");
            var config = HandlerSection.Get();

            var aggregateCatalog = new AggregateCatalog();
            if (config != null)
            {
                foreach (Handler handler in config.HandlerItems)
                {
                    if (handler.Used)
                    {
                        if (File.Exists(ExtensionPath.LocalPath + String.Format("\\{0}.dll", handler.Name)))
                        {
                            var directoryCatalog = new DirectoryCatalog(ExtensionPath.LocalPath, String.Format("{0}.dll", handler.Name));
                            aggregateCatalog.Catalogs.Add(directoryCatalog);
                            Logger.Debug(String.Format("Library for {0} is loaded", handler.Name));
                        }
                        else
                        {
                            Logger.Error(String.Format("Library for {0} is not found", handler.Name));
                        }
                    }
                }
            }
            var container = new CompositionContainer(aggregateCatalog);
            container.ComposeParts(this);
            Logger.Debug("Found {0} modules. Loaded {1} modules.", config.HandlerItems.Count, Transformers.Count());
            Logger.Debug("=== Handler loader end ===");
            return config.HandlerItems.Count == Transformers.Count();
        }
        private void ExchangerInit()
        {
            Logger.Debug("=== Exchanger loader start ===");
            var config = ExchangerSection.Get();
            Exchangers = config.Exchangers.Cast<Exchanger>();
            Storage.Update(Exchangers);
            Logger.Debug("Loaded {0} exchangers", Exchangers.Count());
            Logger.Debug("=== Exchanger loader end ===");
        }

        public void Execute()
        {
            try
            {
                var networkStatus = ConnectionChecker.CheckInternet();
                if (networkStatus != ConnectionChecker.ConnectionStatus.Connected)
                    throw new Exception("No internet connection");
                var data = PrepareData();
                Loader.LoadDataInDb(data);
                Logger.Debug(String.Format("Load {0} records in DataBase", data.Count()));
                Keeper.UpdateList.Clear();
            }
            catch (SqlException ex)
            {
                Logger.Error(ex.Message);
                EmailSender.Send(new[] { "renat.taziyev@walletone.com" }, String.Format("Connection error to database on {0}", ex.Message));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                EmailSender.Send(new[] { "renat.taziyev@walletone.com" }, String.Format("Unexpected exception: {0} <!--{1}-->", ex.Message, ex.StackTrace));
            }
        }

        private string GetAssemblyName(DataTransformer transformer)
        {
            return transformer.Method.Module.Assembly.CustomAttributes.First(a => a.AttributeType.Name == "AssemblyTitleAttribute").ConstructorArguments[0].Value as string;
        }

        private IEnumerable<Valute> PrepareData()
        {
            var exchangers = Exchangers.GroupJoin(Transformers,
                exchanger => exchanger.HandlerName,
                transformer => GetAssemblyName(transformer),
                (exchanger, transformer) => new { Exchanger = exchanger, Transformer = transformer.ElementAtOrDefault(0) });
            foreach (var item in exchangers)
            {
                if (item.Transformer != null)
                {
                    if (DateTime.Now.Subtract(Storage.Storage[item.Exchanger]).TotalHours > 6)
                    {
                        Logger.Debug(String.Format("{0} is work (update {1} valutes)", item.Exchanger.Name, item.Exchanger.Currencies.Count));
                        try
                        {
                            Logger.Debug(String.Format("Download data from {0} begin", item.Exchanger.Url));
                            Downloader downloader = new Downloader(item.Exchanger);

                            Logger.Debug(String.Format("Transform data for {0} begin", item.Exchanger.Name));
                            var valutes = item.Transformer(downloader.Answer);

                            Logger.Debug(String.Format("Load {0} valutes in storage begin", valutes.Count()));
                            var count = Keeper.Update(valutes, item.Exchanger.Currencies.Cast<Valute>());

                            Logger.Debug(String.Format("Handler processed {0} valutes in result", count));
                            Storage.Update(item.Exchanger);
                        }
                        catch (Exception ex)
                        {
                            Logger.Info(String.Format("{0}", ex.Message));
                        }
                    }
                }
                else
                {
                    Logger.Error("{0} without transformer.", item.Exchanger.Name);
                    EmailSender.Send(Emails, String.Format("{0} without transformer.", item.Exchanger.Name));
                }
            }

            return Keeper.UpdateList;
        }
    }
}
