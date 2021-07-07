namespace Dns
{
    /// <summary>
    /// keywords related to
    /// </summary>
    public static class DnsKeywords
    {
        // port dns protocol is on
        public const int DnsPort = 53;
        
        // For Dig response lines that start with ';'
        // line start
        public const string Ls = ";";
        
        // FOr Dig response lines that start with ';;
        // Double line start
        public const string Lls = ";;";
        
        // For Dig answer section response lines that start with '.'
        // Answer section line start
        public const char Als = '.';
        
        // byte value of 'A' type dns request
        public const int AType = 1;
        // byte value of 'AAAA' type dns request
        public const int AAAAType = 28;

        // length of a dns header
        public const int DNSHeaderLength = 12;
    }
}