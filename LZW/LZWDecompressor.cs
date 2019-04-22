using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hifss.LZW
{
    class LZWDecompressor
    {
        private byte _minCodeSize;
        private byte _subBlockSize;
        private List<byte> _data = new List<byte>();
        private List<uint> _codeStream = new List<uint>();
        private List<uint> _decompressedData = new List<uint>();

        public bool Decompress(Stream stream, out uint[] values)
        {
            bool success = true;

            success &= readCodeSize(stream);
            if (!success)
            {
                values = null;
                return false;
            }
            
            success &= readAllSubBlocks(stream);
            success &= getCodes();
            success &= decode();

            values = _decompressedData.ToArray();
            return success;
        }

        private bool decode()
        {
            uint clearCode = _codeStream[0];
            CodeTable codeTable = new CodeTable(_minCodeSize);
            output(_codeStream[1]);
            uint code;

            for (int i = 2; i < _codeStream.Count; i++)
            {
                code = _codeStream[i];

                if (codeTable.HasCode(code))
                {
                    if (codeTable[code] == "EOI")
                    {
                        break;
                    }
                    else if (codeTable[code] == "CC")
                    {
                        codeTable = new CodeTable(_minCodeSize);
                        i++;
                        output(_codeStream[i]);
                    }
                    else
                    {
                        output(codeTable[code].Split(','));
                        string prevCodeValue = codeTable[_codeStream[i - 1]];
                        codeTable.Add($"{prevCodeValue},{codeTable[code].Split(',')[0]}");
                    }
                }
                else
                {
                    string prevCodeValue = codeTable[_codeStream[i - 1]].Split(',')[0]; //K
                    output(codeTable[_codeStream[i - 1]].Split(','));
                    output(prevCodeValue);

                    codeTable.Add($"{codeTable[_codeStream[i - 1]]},{prevCodeValue}");
                }
            }

            return true;
        }

        private void output(params string[] strValues)
        {
            foreach (var strVal in strValues)
            {
                if (strVal != "CC") 
                    _decompressedData.Add(uint.Parse(strVal));
            }
        }

        private void output(params uint[] values)
        {
            _decompressedData.AddRange(values);
        }

        private bool getCodes()
        {
            Decoder decoder = new Decoder(_data.ToArray());
            _codeStream.AddRange(decoder.ReadAllCodes(_minCodeSize));
            
            //TODO Error checking
            return true;
        }

        private bool readAllSubBlocks(Stream stream)
        {
            bool success = true;

            while (readSubBlockSize(stream))
                success &= readSubBlock(stream);

            return success;
        }

        private bool readSubBlock(Stream stream)
        {
            byte[] subBlockData = new byte[_subBlockSize];

            int readBytes = 0;
            while (readBytes < _subBlockSize)
            {
                readBytes += stream.Read(subBlockData, 0, _subBlockSize - readBytes);
            }

            _data.AddRange(subBlockData);

            //TODO Error checking
            return true;
        }

        private bool readSubBlockSize(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1 && readByte != 0)
            { 
                _subBlockSize = (byte)readByte;
            }
            else
                return false;
            return true;
        }

        private bool readCodeSize(Stream stream)
        {
            int readByte = stream.ReadByte();

            if (readByte != -1)
            {
                _minCodeSize = (byte)readByte;
            }
            else
                return false;
            return true;
        }
    }
}
