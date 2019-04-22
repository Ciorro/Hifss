using System;
using System.Collections.Generic;
using System.Text;

namespace Hifss
{
    public class GifImage
    {
        public byte[] Data { get; private set; }
        public uint Delay { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }


        public GifImage(byte[] data, uint x, uint y, uint width, uint height, int delay)
        {
            Data = data;
            Width = width;
            Height = height;
            Delay = (uint)delay;
        }
    }
}
