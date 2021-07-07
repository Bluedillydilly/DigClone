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

        public byte[] request()
        {
            var dataBytes = DnsPacketMaker.DnsPacket(_requestType, _host);
            try
            {
                var client = new UdpClient();
                

                client.Send(dataBytes, dataBytes.Length, "192.168.1.1", 53);
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var echo = client.Receive(ref remoteEndPoint);
                client.Close();
                
                return echo;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override string ToString()
        {
            return $"; <<>> Dig Clone <<>> Dns server: {_dnsServer}, Request Type: {_requestType}, Host: {_host}";
        }
    }
}