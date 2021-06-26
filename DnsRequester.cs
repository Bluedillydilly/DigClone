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

        public override string ToString()
        {
            return $"Dns Server: {_dnsServer}, Request Type: {_requestType}, Host: {_host}";
        }
    }
}