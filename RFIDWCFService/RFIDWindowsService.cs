using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

namespace RFIDWCFService
{
    public class RFIDWindowsService : ServiceBase
    {
        public ServiceHost svh = null;
        public RFIDWindowsService()
        {
            ServiceName = "RFIDService";
        }
        public static void Main(string[] args)
        {
            ServiceBase.Run(new RFIDWindowsService());
        }
        protected override void OnStart(string[] args)
        {
            if (svh != null)
            {
                svh.Close();
            }

            svh = new ServiceHost(typeof(RFID_service));
            svh.Open();
        }
        protected override void OnStop()
        {
            if(svh != null)
            {
                svh.Close();
                svh = null;
            }
        }
    }

    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller proc;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            proc = new ServiceProcessInstaller();
            proc.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "RFIDWindowsService";
            Installers.Add(proc);
            Installers.Add(service);
        }
    }
}