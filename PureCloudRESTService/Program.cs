﻿using System;
using System.ServiceModel.Web;
using System.ServiceProcess;

namespace PureCloudRESTService
{
    static class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "MyService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start(string[] args)
        {
            WebServicesImplementation PureCloudServices = new WebServicesImplementation();
            WebServiceHost _serviceHost = new WebServiceHost(PureCloudServices, new Uri(PureCloudServices.URL));

            _serviceHost.Open();
        }

        private static void Stop()
        {
            
        }
    }
}
