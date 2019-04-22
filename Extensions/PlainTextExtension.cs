using System;
using System.IO;

namespace Hifss.Extensions
{
    internal class PlainTextExtension : Extension
    {
        public override bool Read(Stream stream)
        {
            Console.WriteLine("Extension: Plain text extension. Skipping...");

            //TODO -1 detection
            int bytesToSkip = stream.ReadByte();

            for (int i = 0; i < bytesToSkip; i++)
            {
                stream.ReadByte();
            }

            int currByte = 0;
            while ((currByte = stream.ReadByte()) != 0 || currByte != -1) ;

            return true;
        }
    }
}
