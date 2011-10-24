using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using CapRaffle.Domain.Implementation;

namespace CapRaffle.WinService
{
    partial class RafflingService : ServiceBase
    {
        private System.Timers.Timer timer;
        private bool IsRunning;
        private Thread thread;

        public RafflingService()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(Listen);
            timer.Interval = 10000;
            IsRunning = false;
        }

        protected override void OnStart(string[] args)
        {
            WriteLogEntry("Automatic Raffling has started", EventLogEntryType.SuccessAudit);
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            if (thread.IsAlive)
            {
                thread.Join();
            }
            timer.Dispose();
        }

        private void Listen(object source, ElapsedEventArgs args) {
            if (!IsRunning)
            {
                IsRunning = true;
                thread = new Thread(PerformDrawing); 
                thread.Start();
            }
        }

        private void PerformDrawing() {
            try
            {
                using (var repository = new DrawingRepository())
                {
                    repository.PerformAutomaticDrawing();
                    WriteLogEntry("Performed automatic drawing" + " - " + DateTime.Now);
                }
            }
            catch (Exception e)
            {
                WriteLogEntry("Error occured" + e.Message + " - " + DateTime.Now, EventLogEntryType.Error);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void WriteLogEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            if (!EventLog.SourceExists("Raffling"))
                EventLog.CreateEventSource("Raffling", "Application");
            EventLog.WriteEntry("Raffling", message, type);
        }
    }
}
