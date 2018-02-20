using System;
using System.ServiceProcess;
using System.ServiceModel.Web;

namespace PureCloudRESTService
{
    public partial class RestService : ServiceBase
    {
        public RestService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string port = "8889";
            
            String storageDir = "SampleData";
            if (args != null && args.Length > 0)
            {
                storageDir = args[0];
            }
            if (args != null && args.Length > 1)
            {
                port = args[1];
            }
            Console.WriteLine("Storage directory: " + storageDir);
            Console.WriteLine("Listening on port: " + port);
            WebServicesImplementation DemoServices = new WebServicesImplementation();
            WebServiceHost _serviceHost = new WebServiceHost(DemoServices, new Uri("http://127.0.0.1:" + port));

            _serviceHost.Open();
            //Console.ReadKey();
            //_serviceHost.Close();
        }

        protected override void OnStop()
        {
        }
    }
}
