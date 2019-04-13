using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using NLog;
using W1.CurrencyUpdater.Database;

namespace W1.CurrencyUpdater
{
    public partial class CurrencyUpdater : ServiceBase
    {
        private Timer appTimer, notifyTimer;
        private Application application;
        private System.Threading.Thread thread;

        public CurrencyUpdater()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            
        }
        protected override void OnStop()
        {
            thread.Suspend();
        }

        public void fff ()
        {
            if (thread == null)
            {
                thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartUpdate));
                thread.Start();
                thread.IsBackground = true;
            }
            else
            {
                thread.Resume();
            }
        }

        protected void StartUpdate()
        {
            appTimer = new Timer(new TimeSpan(0, 0, 30).TotalMilliseconds);
            appTimer.Elapsed += new ElapsedEventHandler(Update);
            appTimer.Enabled = true;

            DateTime middleday = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                12, 0, 0
                ).AddDays(1.0);

            notifyTimer = new Timer(middleday.Subtract(DateTime.Now).TotalMilliseconds);
            notifyTimer.Elapsed += new ElapsedEventHandler(Notify);
            notifyTimer.Enabled = true;

            if (application == null)
                application = new Application();
        }
        protected void Update(object sender, ElapsedEventArgs e)
        {
            try
            {
                application.Initialize();
                appTimer.Interval = application.RestartInterval.Milliseconds;
                application.Execute();
            }
            catch (Exception ex)
            {
                Application.Logger.Fatal(ex.Data);
            }
        }
        protected void Notify(object sender, ElapsedEventArgs e)
        {
            try
            {
                EmailSender.Send(application.Emails.ToArray(), Application.Storage.ToString());
                Application.Storage.Update();
            }
            catch (Exception ex)
            {
                Application.Logger.Fatal(ex.Message);
                EmailSender.Send(application.Emails.ToArray(), ex.Message);
            }
        }
    }
}
