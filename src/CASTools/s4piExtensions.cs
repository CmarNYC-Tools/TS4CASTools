
using System;
using System.IO;
using s4pi.ImageResource;
using s4pi.Interfaces;

namespace XMODS
{
    public static class s4piExtensions
    {

        public static Stream ToDDS(this AResource r) => r switch
        {
            null => (Stream)null,
            RLEResource rle => rle.ToDDS(),
            DSTResource dst => dst.ToDDS(),
            _ => throw new NotSupportedException($"Unable to convert {r.GetType().Name} to DDS.")

        };
    }
}