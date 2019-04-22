using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hifss
{
    class ColorTable
    {
        public List<Color> Colors { get; private set; } = new List<Color>();

        public Color this[int index]
        {
            get { return Colors[index]; }
        }

        public bool ReadColors(Stream stream, uint colorCount)
        {
            for (int i = 0; i < colorCount; i++)
            {
                try
                {
                    byte[] color = new byte[3];
                    int readBytes = stream.Read(color, 0, 3);

                    Colors.Add(new Color(color[0], color[1], color[2]));
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
