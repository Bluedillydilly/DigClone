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
        /// Simple wrapper of sorts. 
        /// </summary>
        /// <param name="args">cli args</param>
        static void Main(string[] args)
        {
            if (args.Length < minArgs || maxArgs < args.Length)
            {
                Console.WriteLine("Invalid number of parameters.");
                Usage();
            }
                
            var (hostName, dnsServer, requestType) = ParseCliArgs(args);
            var dnsRequester = new DnsRequester(hostName, dnsServer, requestType);
            Console.WriteLine(dnsRequester);
            var response = dnsRequester.request();

            var respReader = new DnsResponseReader(response);
            respReader.read();
            
        }

        /// <summary>
        /// Prints usage and exits program. If called cli args error has occured
        /// </summary>
        static void Usage()
        {
            var usage = "dotnet run [<DnsServer>] [<Dns Query Type>] hostname";
            Console.WriteLine(usage);
            Environment.Exit(-1);
        }

        /// <summary>
        /// Parsers the cli args and returns target hostname, dns server to use, and dns request type
        /// </summary>
        /// <param name="argv"></param>
        /// <returns>hostname, dns server address, dns request type</returns>
        private static (string, string, DnsRequestType) ParseCliArgs(string[] argv)
        {
            string dnsServer = DefaultDnsServer();
            DnsRequestType requestType = DnsRequestType.A;
            
            // get hostname (argc 1, 2, 3)
            var host = argv[^1];

            // get request type (argc 2, 3)
            if (argv.Length > 1)
            {
                switch (argv[^2].ToUpper())
                {
                    case "A":
                        requestType = DnsRequestType.A;
                        break;
                    case "AAAA" :
                        requestType = DnsRequestType.AAAA;
                        break;
                    default:
                        Console.WriteLine("Invalid Dns request type specified. Please select between 'A' and 'AAAA' type");
                        Usage();
                        break;
                }
                // get DNS server (argc 3)
                if (argv.Length > 2)
                {
                    dnsServer = argv[^3];
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
        /// Gets the default dns server to use to start the dns request with
        /// </summary>
        /// <returns>default local dns server</returns>
        static string DefaultDnsServer()
        {
            var adapters  = NetworkInterface.GetAllNetworkInterfaces();
            string address = "";
            foreach (NetworkInterface adapter in adapters)
            {
                // determine if the interface is up, and not loopback
                if (adapter.OperationalStatus != OperationalStatus.Up ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                    adapter.Name.StartsWith("vEthernet")) continue;
                // for each valid adapter, get the IPProperties, and the DNS Addresses
                var adapterDnsAddresses = adapter.GetIPProperties().DnsAddresses;
                foreach (var addr in adapterDnsAddresses)
                {
                    // make sure the DNS server is valid and return it
                    // try connecting to the server 
                    address = addr.ToString();
                }


            }
            
            return address;
        }
    }
}