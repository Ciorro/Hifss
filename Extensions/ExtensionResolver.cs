using System;
using System.Collections.Generic;
using System.Text;

namespace Hifss.Extensions
{
    class ExtensionResolver
    {
        private Dictionary<int, Type> _extCodeMap = new Dictionary<int, Type>()
        {
            { 249, typeof(GraphicsControlExtension)}, //Graphic control extension
            { 255, typeof(ApplicationExtension)}, //Application extension
            { 254, typeof(CommentExtension)}, //Comment extension
            { 1, typeof(PlainTextExtension)} //Plain text extension
        };

        public Extension GetExtension(int extensionLabel)
        {
            return (Extension)Activator.CreateInstance(_extCodeMap[extensionLabel]);
        }
    }
}
