using System;

namespace Dns
{
    public static class Extensions
    {
        public static void Deconstruct(this string[] arr, out string hostname, out string domainName)
        {
            hostname = arr[0];
            domainName = arr[1];
        }

        public static void Deconstruct(this byte[] respArr, out byte[] header, out byte[] body)
        {
            header = new byte[DnsKeywords.DNSHeaderLength];
            Array.Copy(respArr, 0, header, 0, header.Length);
            
            body = new byte[respArr.Length - DnsKeywords.DNSHeaderLength];
            Array.Copy(respArr, DnsKeywords.DNSHeaderLength, body, 0, respArr.Length - DnsKeywords.DNSHeaderLength);

        }
        
        
        public static void Deconstruct(this byte[] body, out byte[] query, out byte[] answers, out byte[] authNameServers, out byte[] addRecords)
        {
            var offset = 0;
            var localOffset = 0;
            // query
            foreach (var b in body)
            {
                if (b == 0x00)
                {
                    break;
                }
                localOffset += 1;

            }

            localOffset += 2 + 2; // type and class offsets (2 byte each)
            //Console.WriteLine("LAST QUERY BYTE: {0}", body[localOffset]);

            localOffset++;
            query = new byte[localOffset];
            offset = localOffset;
            Array.Copy(body, 0, query, 0, localOffset);
            //DnsPacketMaker.printByteArray(query);
            
            
            // answers
            localOffset = offset;
            //Console.WriteLine(body[localOffset]);
            
            answers = new byte[localOffset]; 

            // auth name servers
            localOffset = 0;

            authNameServers = new byte[localOffset]; 

            // additional records
            localOffset = 0;
            
            addRecords = new byte[localOffset]; 

        }
        
    }
}