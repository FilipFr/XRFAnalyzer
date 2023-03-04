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

namespace XRFAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App() {
            ProcessStartInfo processInfo = new ProcessStartInfo("Processes/Server/XRFAnalyzerGrpcServer.exe");
            // Configure the process using the StartInfo properties.
            processInfo.CreateNoWindow = true;
            Process.Start(processInfo);
        }
    }
        
}

