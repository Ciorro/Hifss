using System.Collections.Generic;
using System.IO;

namespace Hifss
{
    public class Gif
    {
        public List<GifImage> Frames = new List<GifImage>();

        public Gif()
        {
        }

        public Gif(string path)
        {
            LoadFromFile(path);
        }

        public bool LoadFromFile(string path)
        {
            return LoadFromMemory(File.ReadAllBytes(path));
        }

        public bool LoadFromMemory(byte[] data)
        {
            bool success = false;

            using (MemoryStream ms = new MemoryStream(data))
            {
                GifLoader loader = new GifLoader(ms);
                success = loader.LoadGif();

                if (success)
                    Frames = loader.GetImages();
            }

            return success;
        }
    }
}
