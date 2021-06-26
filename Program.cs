using System;
using System.Net.NetworkInformation;

namespace Dns
{
    static class Program
    {
        // minimum number of CLI args required
        private static int minArgs = 1;
        
        // maximum number of CLI args allowed
        private static int maxArgs = 3;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < minArgs || maxArgs < args.Length)
            {
                Console.WriteLine("Invalid number of parameters.");
                Usage();
            }
            
            foreach (var param in args)
            {
                Console.WriteLine(param);
            }

            (string hostName,
                    string dnsServer,
                    DnsRequestType requestType
            ) = ParseCliArgs(args);
            var dnsRequester = new DnsRequester(hostName, dnsServer, requestType);

        }

        /// <summary>
        /// 
        /// </summary>
        static void Usage()
        {
            var usage = "dotnet run [<DnsServer>] [<Dns Query Type>] hostname";
            Console.WriteLine(usage);
            Environment.Exit(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        static (string, string, DnsRequestType) ParseCliArgs(string[] argv)
        {
            string dnsServer = DefaultDnsServer();
            DnsRequestType requestType = DnsRequestType.A;
            
            // get hostname (argc 1, 2, 3)
            var host = argv[-1];

            // get request type (argc 2, 3)
            if (argv.Length > 1)
            {
                switch (argv[-2].ToUpper())
                {
                    case "A":
                        requestType = DnsRequestType.A;
                        break;
                    case "AAAA" :
                        requestType = DnsRequestType.AAAA;
                        break;
                    case "CNAME":
                        requestType = DnsRequestType.CNAME;
                        break;
                    default:
                        Console.WriteLine("Invalid Dns request type specified.");
                        Usage();
                        break;
                }
                // get DNS server (argc 3)
                if (argv.Length > 2)
                {
                    dnsServer = argv[-3];
                }
            }
            else
            {
                requestType = DnsRequestType.A;
                dnsServer = DefaultDnsServer();
            }

            return (host, dnsServer, requestType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static string DefaultDnsServer()
        {
            NetworkInterface[] adapters  = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters) 
            {
                // determine if the interface is up, and not loopback
                // for each valid adapter, get the IPPropties, and the DNS Addresses
                    // make sure the DNS server is valid and return it
            }
            
            return "";
        }
    }


}