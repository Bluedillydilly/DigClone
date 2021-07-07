using System;
using System.Net;
using System.Net.Sockets;

namespace Dns
{
    /// <summary>
    /// Makes a udp dns request
    /// </summary>
    public class DnsRequester
    {
        private readonly string _host;
        private readonly string _dnsServer;
        private readonly DnsRequestType _requestType;
        
        /// <summary>
        /// Constructor for a dns requester
        /// </summary>
        /// <param name="host">address to get the ipaddress of - target of dns request</param>
        /// <param name="dnsServer">what local dns server to initiate request with</param>
        /// <param name="requestType">type of dns request A/AAAA</param>
        public DnsRequester(string host, string dnsServer, DnsRequestType requestType)
        {
            _host = host;
            _dnsServer = dnsServer;
            _requestType = requestType;
        }

        /// <summary>
        /// Send a dns request and returns the response to the request
        /// </summary>
        /// <returns>response of the dns request</returns>
        public byte[] request()
        {
            var dataBytes = DnsPacketMaker.DnsPacket(_requestType, _host);
            try
            {
                var client = new UdpClient();
                
                client.Send(dataBytes, dataBytes.Length, _dnsServer, DnsKeywords.DnsPort);
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

        /// <summary>
        /// String rep of a dns requester
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"; <<>> Dig Clone <<>> Dns server: {_dnsServer}, Request Type: {_requestType}, Host: {_host}";
        }
    }
}