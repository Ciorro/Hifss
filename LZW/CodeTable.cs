using System;
using System.Collections.Generic;
using System.Text;

namespace Hifss.LZW
{
    class CodeTable
    {
        private Dictionary<uint, string> _table = new Dictionary<uint, string>();
        private uint _index = 0;

        public string this[uint i]
        {
            get
            {
                return _table[i];
            }
        }

        public CodeTable(uint lzwMinCodeSize)
        {
            initialize(lzwMinCodeSize);
        }
        
        public void Add(string value)
        {
            _table.Add(_index, value);
            _index++;
        }

        public bool HasCode(uint value)
        {
            return _table.ContainsKey(value);
        }

        private void initialize(uint lzwMinCodeSize)
        {
            _table.Clear();

            for (_index = 0; _index < Math.Pow(2, lzwMinCodeSize); _index++) 
            {
                _table.Add(_index, _index.ToString());
            }

            Add("CC");
            Add("EOI");
        }
    }
}
