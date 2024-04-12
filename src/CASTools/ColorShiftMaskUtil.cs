using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DotNetDDS;
namespace XMODS
{
    public static class ColorShiftMaskUtil
    {
        public static Stream ToColorShiftMask(this Bitmap bitmap)
        {
            Bitmap Resize(Bitmap src, Size sz)
            {
                if (src.Size == sz) return src;
                var dst = new Bitmap(sz.Width, sz.Height);
                using var g = Graphics.FromImage(dst);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(src, 0, 0, sz.Width, sz.Height);
                return dst;
            }
            byte[] ExtractRed(Bitmap src)
            {
                var dta = new byte[src.Width * src.Height];
                var lck = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                for (int i = 0; i < dta.Length; i++) dta[i] = Marshal.ReadByte(lck.Scan0, i * 4);
                src.UnlockBits(lck);
                return dta;
            }
            var mipSizes = (from i in Enumerable.Range(0, 20)
                            select new Size(bitmap.Width >> i, bitmap.Height >> i)).TakeWhile(x => x.Width >= 4 && x.Height >= 4).ToArray();
            var hdr = new DDS_HEADER
            {
                dwSize = DDS_HEADER.SIZE,
                dwFlags = DDS_HEADER_FLAGS.DDSD_MIPTEXTURE | DDS_HEADER_FLAGS.DDSD_PITCH,
                dwPitchOrLinearSize = (uint)bitmap.Width,
                dwDepth = 0,
                dwWidth= (uint)bitmap.Width,
                dwHeight = (uint)bitmap.Height,
                dwMipMapCount = (uint)mipSizes.Length,
                ddspf = new DDS_PIXELFORMAT
                {
                    dwABitMask = 0,
                    dwRBitMask = 0xff,
                    dwGBitMask = 0xff,
                    dwBBitMask = 0xff,
                    dwRGBBitCount = 8,
                    dwFlags = DDS_PIXELFORMAT_FLAGS.DDPF_LUMINANCE,
                    dwSize = DDS_PIXELFORMAT.SIZE
                }
            };
            hdr.dwMipMapCount = (uint)mipSizes.Length;
            var ms = new MemoryStream();
            ms.Write(DDS_HEADER.MAGIC,0,4);
            byte[] hdrb = hdr.ToBytes();
            ms.Write(hdrb, 0, hdrb.Length);
            foreach (var sz in mipSizes)
            {
                var mip = ExtractRed(Resize(bitmap, sz));
                ms.Write(mip, 0, mip.Length);
            }
            ms.Position = 0;
            return ms;
        }
    }
}