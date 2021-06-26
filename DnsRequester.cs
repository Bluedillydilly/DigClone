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
    }
}