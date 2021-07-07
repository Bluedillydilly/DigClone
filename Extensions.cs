using System;

namespace Dns
{
    /// <summary>
    /// Deconstruction extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Deconstruct a '.' split address into the hostname and domain name
        /// </summary>
        /// <param name="arr">split address</param>
        /// <param name="hostname">hostname part of the address</param>
        /// <param name="domainName">domain name part of the address</param>
        public static void Deconstruct(this string[] arr, out string hostname, out string domainName)
        {
            hostname = arr[0];
            domainName = arr[1];
        }

        /// <summary>
        /// Deconstruct a dns response byte array into the a header and body
        /// </summary>
        /// <param name="respArr">dns response byte array</param>
        /// <param name="header">header part of dns packet</param>
        /// <param name="body">body part of dns packet</param>
        public static void Deconstruct(this byte[] respArr, out byte[] header, out byte[] body)
        {
            header = new byte[DnsKeywords.DNSHeaderLength];
            Array.Copy(respArr, 0, header, 0, header.Length);
            
            body = new byte[respArr.Length - DnsKeywords.DNSHeaderLength];
            Array.Copy(respArr, DnsKeywords.DNSHeaderLength, body, 0, respArr.Length - DnsKeywords.DNSHeaderLength);

        }
        
        /// <summary>
        /// Deconstruct the body of a dns response byte array into the various parts. INCOMPLETE. NOT FINISH. WIP
        /// </summary>
        /// <param name="body">The body of the dns packet</param>
        /// <param name="query">bytes related to the initial dns query</param>
        /// <param name="answers">bytes answers to the dns query </param>
        /// <param name="authNameServers">authorital servers in the dns query process</param>
        /// <param name="addRecords"></param>
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