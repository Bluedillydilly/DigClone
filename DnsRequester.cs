using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Dns
{
    public class DnsRequester
    {
        private readonly string _host;
        private readonly string _dnsServer;
        private readonly DnsRequestType _requestType;
        
        public DnsRequester(string host, string dnsServer, DnsRequestType requestType)
        {
            _host = host;
            _dnsServer = dnsServer;
            _requestType = requestType;
        }

        public void request()
        {
            var dataBytes = DnsPacketMaker.DnsPacket(_requestType, _host);
            try
            {
                Console.WriteLine($"Connecting to DNS server { _dnsServer }...");
                var tcpClient = new TcpClient( _dnsServer, DnsKeywords.DnsPort );
                tcpClient.Client.Send(dataBytes);
                //Console.WriteLine("Request sent");
                //var tcpClient = new TcpClient("time.nist.gov", 13);
                var reader = new StreamReader(tcpClient.GetStream());
                Console.WriteLine("Reading new data");
                while (!reader.EndOfStream)
                {
                    Console.WriteLine("line: {0}",reader.ReadLine());
                }
                reader.Close();
                tcpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override string ToString()
        {
            return $"Dns Server: {_dnsServer}, Request Type: {_requestType}, Host: {_host}";
        }
    }
}