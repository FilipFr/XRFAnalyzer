using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace XRFAnalyzer.Processes.Client
{
    internal class XRFAnalyzerGrpcClient
    {
        static GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:51001");
        XRFAnalyzerService.XRFAnalyzerServiceClient client = new XRFAnalyzerService.XRFAnalyzerServiceClient(channel);
    }
}
