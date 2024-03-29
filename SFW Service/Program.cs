﻿using System.ServiceProcess;

namespace SFW_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SFWService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
