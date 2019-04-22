using Hifss.Extensions;
using Hifss.LZW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hifss
{
    class GifLoader
    {
        private Stream _stream;

        private List<GifImage> _images = new List<GifImage>();
        private List<Extension> _extensions = new List<Extension>();

        public GifHeader Header;
        public ScreenDescriptor ScreenDescriptor;
        public ColorTable GlobalColorTable;

        public GifLoader(Stream stream)
        {
            _stream = stream;
        }

        public List<GifImage> GetImages()
        {
            return _images;
        }

        public bool LoadGif()
        {
            bool success = true;

            success &= readHeader();
            success &= readScreenDescriptor();

            if (ScreenDescriptor.HasGlobalColorTable)
                success &= readGlobalColorTable();

            GifReader reader = new GifReader(_stream);
            int blockType;
            while ((blockType = reader.ReadType()) != -1)
            {
                if (blockType == GifReader.Trailer)
                    break;
                else if (blockType == GifReader.ExtensionIntroducer)
                    _extensions.Add(reader.ReadExtension());
                else if (blockType == GifReader.ImageDescriptor)
                    loadImage();
            }

            return success;
        }

        private bool readHeader()
        {
            Header = new GifHeader();
            return Header.Read(_stream);
        }

        private bool readScreenDescriptor()
        {
            ScreenDescriptor = new ScreenDescriptor();
            return ScreenDescriptor.Read(_stream);
        }

        private bool readGlobalColorTable()
        {
            GlobalColorTable = new ColorTable();
            return GlobalColorTable.ReadColors(_stream, ScreenDescriptor.EntryCount);
        }

        private void loadImage()
        {
            ImageDescriptor imageDescriptor = new ImageDescriptor();
            imageDescriptor.Read(_stream);

            ColorTable localColorTable = GlobalColorTable;

            if (imageDescriptor.HasLocalColorTable)
            {
                localColorTable = new ColorTable();
                localColorTable.ReadColors(_stream, imageDescriptor.EntryCount);
            }

            uint[] colorIndexes;
            LZWDecompressor lzw = new LZWDecompressor();
            lzw.Decompress(_stream, out colorIndexes);

            createImage(colorIndexes, imageDescriptor, localColorTable);
        }

        private void createImage(uint[] colorIndexes, ImageDescriptor imageDescriptor, ColorTable localColorTable)
        {
            uint localX = imageDescriptor.X;
            uint localY = imageDescriptor.Y;
            uint localW = imageDescriptor.Width;
            uint localH = imageDescriptor.Height;

            //Every pixel consists of 4 bytes: R, G, B, A values
            byte[] data = new byte[localW * localH * 4];

            int transparentColorIndex = -1;
            int delay = 0;

            if (_extensions.Last() is GraphicsControlExtension)
            {
                GraphicsControlExtension gce = _extensions.Last() as GraphicsControlExtension;
                delay = gce.Delay;
                transparentColorIndex = gce.TransparentColorIndex;
            }

            for (int i = 0; i < data.Length; i += 4)
            {
                Color color = localColorTable[(int)colorIndexes[i / 4]];
                data[i + 0] = color.R;
                data[i + 1] = color.G;
                data[i + 2] = color.B;

                if (colorIndexes[i / 4] == transparentColorIndex)
                    data[i + 3] = 0;
                else
                    data[i + 3] = byte.MaxValue;
            }

            _images.Add(new GifImage(data, localX, localY, localW, localH, delay));
        }
    }
}
