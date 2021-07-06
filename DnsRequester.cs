using System;
using System.IO;
using System.Net;
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
                //var tcpClient = new TcpClient( _dnsServer, DnsKeywords.DnsPort );
                var client = new UdpClient();
                
                Console.Write("Sending: ");
                DnsPacketMaker.printByteArray(dataBytes);

                client.Send(dataBytes, dataBytes.Length, "192.168.1.1", 53);
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var echo = client.Receive(ref remoteEndPoint);
                DnsPacketMaker.printByteArray(echo);
                client.Close();


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