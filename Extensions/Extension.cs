using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hifss.Extensions
{
    abstract class Extension
    {
        public abstract bool Read(Stream stream);
    }
}
