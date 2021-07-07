using System;
using System.Text;

namespace Dns
{
    /// <summary>
    /// Parsers and outputs a dns request response
    /// </summary>
    public class DnsResponseReader
    {
        private byte[] _header;
        private byte[] _body;
        private byte[] _query;

        private string _hostname;

        private int _answerCount;
        private int _authCount;
        private int _addRRCount;
        
        private int _offset = 0;

        public DnsResponseReader(byte[] resp)
        {
            (_header, _body) = resp;
            (_query, _, _, _) = _body;
            //DnsPacketMaker.printByteArray(_header);
        }

        
        /// <summary>
        /// Parses and prints out a dns request response in a similar layout to *nix dig
        /// </summary>
        public void read()
        {
            Console.WriteLine(";; Got answer:");
            // print header info
            header();
            
            // print Query
            question();
            
            // print answers
            answer();
            
            // skip authorital servers
            skipAuth();
            
            // print additional RRs
            addRRs();
        }

        /// <summary>
        /// Prints out header response info
        /// </summary>
        private void header()
        {
            var status = "NOERROR";
            var id = _header[0] << 8;
            id += _header[1];
            if ( (_header[3] & 0x0f) != 0x00)
            {
                status = "ERROR";
            }
            Console.WriteLine(";; ->>HEADER<<- status: {0}, id: {1}", status, id);

            var queryCount = (_header[4] << 8) + _header[5];
            _answerCount = (_header[6] << 8) + _header[7];  // answer RRs
            _authCount = (_header[8] << 8) + _header[9];  // auth RRs
            _addRRCount = (_header[10] << 8) + _header[11]; // additional RRs
            Console.WriteLine(";; QUERY: {0}, AUTH: {2}, ANSWER: {1}", queryCount, _answerCount, _authCount);
        }

        /// <summary>
        /// Prints out initial query/question response info
        /// </summary>
        private void question()
        {
            var sb = new StringBuilder();
            for (var index = 1; index < _query.Length; index++)
            {
                var b = _query[index];
                if (b == 0x03)
                {
                    sb.Append('.');
                }
                else if (b == 0x00)
                {
                    break;
                }
                else
                {
                    sb.Append(Convert.ToChar(b));
                }
            }

            var typeCode = (_query[^4] << 8) + _query[^3];
            var requestType = "A";
            if (typeCode != 1)
            {
                requestType = $"ERROR { typeCode }";
            }
            var classCode = (_query[^2] << 8) + _query[^1];
            var requestClass = "IN";
            if (classCode != 1)
            {
                requestClass = $"ERROR { classCode }";
            }

            _hostname = sb.ToString();
            Console.WriteLine(";; QUESTION SECTION:");
            Console.WriteLine(";{0}.\t {1}\t {2}", _hostname, requestClass, requestType);
            
            Console.WriteLine();
        }

        /// <summary>
        /// Prints out answers to the dns request
        /// </summary>
        private void answer()
        {
            var offset = _query.Length;
            Console.WriteLine(";; ANSWER SECTION:");
            for (var i = 0; i < _answerCount; i++)
            {
                // get answer name
                string name;
                if (_body[offset] == 0xc0)
                {
                    name = _hostname;
                    offset += 2;
                }
                else
                {
                    name = "ERROR";
                    Console.WriteLine("NON PTR NAME");
                }
                
                // get answer request type
                string requestType;
                switch ( (_body[offset] << 8) + _body[offset + 1])
                {
                    case 0x01:
                        requestType = "A";
                        break;
                    case 0x1c:
                        requestType = "AAAA";
                        break;
                    case 0x05:
                        requestType = "CNAME";
                        break;
                    default:
                        requestType = "ERROR";
                        Console.WriteLine("INVALID REQUEST TYPE {0}", (_body[offset] << 8) + _body[offset + 1]);
                        break;
                }

                offset += 2;
                
                
                // get answer class type
                var classType = "IN";
                if ((_body[offset] << 8) + _body[offset + 1] != 0x01)
                {
                    classType = "ERROR";
                }

                offset += 2;


                offset += 4; // skip time to live

                // get data length
                var dataLength = (_body[offset] << 8) + _body[offset + 1];
                offset += 2;
                // get answer address
                var ipAddr = new StringBuilder();
                if (requestType == "A")
                {
                    for (int j = 0; j < dataLength; j++)
                    {
                        ipAddr.Append(_body[offset]);
                        ipAddr.Append('.');
                        offset++;
                    }
                }
                else if (requestType == "AAAA")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (_body[offset] != 0x00)
                        {
                            ipAddr.Append(_body[offset].ToString("X2"));
                        }

                        if (j % 2 == 1)
                        {
                            ipAddr.Append(':');

                        }
                        offset++;
                    }

                    offset += 7;
                    ipAddr.Append(':');

                    ipAddr.Append(_body[offset].ToString("X2"));
                    offset++;
                }



                _offset = offset;

                Console.WriteLine("{0}.\t {1}\t {2}\t {3}", name, classType, requestType, ipAddr);
            }
            
            
        }

        /// <summary>
        /// Skips authorital server response info
        /// </summary>
        private void skipAuth()
        {
            var offset = _offset;
            for (int i = 0; i < _authCount; i++)
            {
                // each auth variable
                var authLength = 0;
                // name bytes
                authLength += 1;
                // type bytes
                authLength += 2;
                // class bytes
                authLength += 2;
                // time to live bytes
                authLength += 4;
                // data length bytes
                var dataLength = (_body[offset + authLength] << 8) + _body[offset + authLength + 1];
                authLength += 2;
                // name server bytes
                authLength += dataLength;
                offset += authLength;
            }

            _offset = offset;
            
        }

        /// <summary>
        /// Prints out the additional resource record responses. Servers used in the hops
        /// </summary>
        private void addRRs()
        {
            var offset = _offset;
            
            for (var i = 0; i < _addRRCount; i++)
            {
                Console.WriteLine(_body[offset]);

                // get answer name
                string name;
                if ( (_body[offset] & 0xc0) == 0xc0)
                {
                    name = "root server";
                }
                else
                {
                    name = "ERROR";
                }
                offset += 2;

                
                // get answer request type
                string requestType;
                switch ( (_body[offset] << 8) + _body[offset + 1])
                {
                    case 0x01:
                        requestType = "A";
                        break;
                    case 0x1c:
                        requestType = "AAAA";
                        break;
                    case 0x05:
                        requestType = "CNAME";
                        break;
                    default:
                        requestType = "ERROR";
                        Console.WriteLine("INVALID REQUEST TYPE {0}", (_body[offset] << 8) + _body[offset + 1]);
                        break;
                }

                offset += 2;
                
                
                // get answer class type
                var classType = "IN";
                if ((_body[offset] << 8) + _body[offset + 1] != 0x01)
                {
                    classType = "ERROR";
                }

                offset += 2;


                offset += 4; // skip time to live

                // get data length
                var dataLength = (_body[offset] << 8) + _body[offset + 1];
                offset += 2;
                // get answer address
                var ipAddr = new StringBuilder();
                if (requestType == "A")
                {
                    for (int j = 0; j < dataLength; j++)
                    {
                        ipAddr.Append(_body[offset]);
                        ipAddr.Append('.');
                        offset++;
                    }
                }
                else if (requestType == "AAAA")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (_body[offset] != 0x00)
                        {
                            ipAddr.Append(_body[offset].ToString("X2"));
                        }

                        if (j % 2 == 1)
                        {
                            ipAddr.Append(':');

                        }
                        offset++;
                    }

                    offset += 7;
                    ipAddr.Append(':');

                    ipAddr.Append(_body[offset].ToString("X2"));
                    offset++;
                }



                _offset = offset;

                Console.WriteLine("{0}.\t {1}\t {2}\t {3}", name, classType, requestType, ipAddr);
            }
            
            
        }
    }
}