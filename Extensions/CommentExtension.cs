using System;
using System.IO;

namespace Hifss.Extensions
{
    internal class CommentExtension : Extension
    {
        public override bool Read(Stream stream)
        {
            Console.WriteLine("Extension: Comment extension. Skipping...");

            int bytesToSkip = stream.ReadByte();
            for (int i = 0; i < bytesToSkip + 1; i++)
            {
                stream.ReadByte();
            }

            return true;
        }
    }
}
