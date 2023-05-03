using ControlzEx.Theming;
using Grpc.Net.Client;
using MahApps.Metro.Theming;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XRFAnalyzer.Processes.Client;
using XRFAnalyzer.ViewModels;

namespace XRFAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App() 
        {
            XRFAnalyzerGrpcClient xRFAnalyzerGrpcClient = new XRFAnalyzerGrpcClient();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            foreach (Process process in Process.GetProcessesByName("XRFAnalyzerGrpcServer"))
            {
                process.Kill();
            }
            //ProcessStartInfo processInfo = new ProcessStartInfo("Processes/Server/XRFAnalyzerGrpcServer.exe");
            //// Configure the process using the StartInfo properties.
            //processInfo.CreateNoWindow = true;
            //processInfo.UseShellExecute = false;
            //Process.Start(processInfo);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            foreach (Process process in Process.GetProcessesByName("XRFAnalyzerGrpcServer"))
            {
                process.Kill();
            }
            base.OnExit(e);
        }
        
}
        
}

