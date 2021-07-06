using System;
using System.Linq;
using System.Text;

namespace Dns
{
    public static class DnsPacketMaker
    {
        public static byte[] DnsPacket(DnsRequestType rType, string address)
        {   
            // DNS header 12 bytes
            var headerBytes = new byte[12];
                // Transaction ID 2 bytes
            headerBytes[0] = 0x2a;
            headerBytes[1] = 0x9c;
                    // anything you want, server will respond with same ID
                // 7 Flags 2 bytes
            headerBytes[2] = 0x01;
            headerBytes[3] = 0x00;
            // Questions 2 bytes
            headerBytes[4] = 0x00;
            headerBytes[5] = 0x01;
                    // number of queries?
                // Answer RRs 2 bytes (?)
            headerBytes[6] = 0x00;
            headerBytes[7] = 0x00;
                // Authority RRs 2 bytes (?)
            headerBytes[8] = 0x00;
            headerBytes[9] = 0x00;
                // Additional RRs 2 bytes (?)
            headerBytes[10] = 0x00;
            headerBytes[11] = 0x00;
            
            // Query
            byte[] queryBytes;
            var (hostname, domainName) = address.Split('.');
                // address
                    // address length 1 byte
                        // snapchat.com = 8 (snapchat)
            var addressLengthBytes = new byte[] { Convert.ToByte(hostname.Length) };
                    // hostname variable bytes
            var hostnameBytes = Encoding.ASCII.GetBytes(hostname);
                        // snapchat
                    // period 0x03 1 byte
            var periodByte = new byte[] { 0x03 };
                    // domain name bytes
                        // com
            var domainNameBytes = Encoding.ASCII.GetBytes(domainName);
                    // null byte 1 byte
                        // acts as null terminator
            var nullByte = new byte[] { 0x00 };
                        // record type 2 byte
            var recordType = new byte[2];
            recordType[0] = 0x00;

            switch (rType)
            {
                case DnsRequestType.A: // 0x00 0x01 == A type record
                    recordType[1] = 0x01;
                    break;
                case DnsRequestType.AAAA: // 0x00 0x1c == AAAA type record
                    recordType[1] = 0x1c;
                    break;
                default:
                    Console.WriteLine($"INVALID dns request type: { rType }");
                    return Array.Empty<byte>();
            }
            
            // class type 2 bytes (?)
            // 0x00 0x01 == IN class type
            var classBytes = new byte[2] { 0x00, 0x01 };

            queryBytes = mergeByteArrays(new byte[][]
            {
                addressLengthBytes, hostnameBytes, periodByte, domainNameBytes,
                nullByte, recordType, classBytes
            });

            var dnsRequestBytes = mergeByteArrays(new byte[][] { headerBytes, queryBytes });
            
            //TODO: verify correct packet bytes
            //Console.Write("DNS Request: ");
            //printByteArray(dnsRequestBytes);
            
            return dnsRequestBytes;
        }

        private static void Deconstruct(this string[] arr, out string hostname, out string domainName)
        {
            hostname = arr[0];
            domainName = arr[1];
        }

        public static void printByteArray(byte[] arr)
        {
            Console.WriteLine( BitConverter.ToString(arr).Replace('-', ' ') );
        }

        private static byte[] mergeByteArrays(byte[][] arrs)
        {
            var bigArr = new byte[arrs.Aggregate(0, (total, elem) => total + elem.Length)];
            var offset = 0;
            foreach (var t in arrs)
            {
                t.CopyTo(bigArr, offset);
                offset += t.Length;
            }

            return bigArr;
        }
    }
}