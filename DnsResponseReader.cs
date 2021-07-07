using System;
using System.Diagnostics;
using System.Text;

namespace Dns
{
    public class DnsResponseReader
    {
        private byte[] _header;
        private byte[] _body;
        private byte[] _query;

        private string _hostname;

        private int _answerCount;
        private int _addRRCount;
        
        private int _offset = 0;

        public DnsResponseReader(byte[] resp)
        {
            (_header, _body) = resp;
            (_query, _, _, _) = _body;
            //DnsPacketMaker.printByteArray(_header);
        }

        
        
        public void read()
        {
            Console.WriteLine(";; Got answer:");
            // print header info
            header();
            
            // print Query
            question();
            
            // print answers
            answer();
        }

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
            _addRRCount = (_header[10] << 8) + _header[11]; // additional RRs
            Console.WriteLine(";; QUERY: {0}, ANSWER: {1}", queryCount, _answerCount);
        }

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

        private void answer()
        {
            var offset = _query.Length;
            Console.WriteLine(_body[offset]);
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
                        requestType = "AAAA"
                }
                
                
                // get answer class type
                var classType = "IN";
                if (_body[offset] != 0x00 || _body[offset + 1] != 0x01)
                {
                    classType = "ERROR";
                }

                offset += 2;
                
               
                
                // get answer address
                
                Console.WriteLine("{0}.\t {1}\t {2}\t {3}", name, classType, name, name);
            }
            
            
        }

        private void addRRs()
        {
            for (var i = 0; i < _addRRCount; i++)
            {
                Console.WriteLine("{0}.\t {1}\t {2}\t {3}");
            }
        }
    }
}