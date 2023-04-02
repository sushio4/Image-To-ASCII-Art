using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace AsciiArt
{
    class ASCIIMaker
    {

        public ASCIIMaker()
        { }

        public ASCIIMaker(Bitmap bmp)
        {
            awidth = 10;
            lock(locker)
            {
                original = bmp;
                width = bmp.Width;
                height = bmp.Height;
            }
            aheight = awidth * height / width;
        }

        public void MakeGray()
        {
            gray = new byte[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _MakeGray(x, y, original.GetPixel(x,y));
                }
            }
        }

        public void AdjustContrast()
        {
            byte minInt = 255;
            byte maxInt = 0;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    minInt = Math.Min(minInt, gray[x + width * y]);
                    maxInt = Math.Max(maxInt, gray[x + width * y]);
                }
            byte factor = (byte)(255 / (maxInt - minInt));
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int g = (gray[x + y * width] - minInt) * factor;
                    g = (int)(g * contrastFactor + 128 * (1 - contrastFactor));
                    gray[x + y * width] = (byte)Math.Clamp(g, 0, 255);
                }
        }

        public void _MakeGray(int x, int y, Color c)
        {
            byte g = (byte)(c.A * (c.R + c.G + c.B) / (3 * 255));
            if (invert) g = (byte)(255 - g);
            gray[x + y * width] = g;
        }

        public void SetWidth(int w)
        {
            awidth = w;
            lock(locker)
            {
                if(original != null)
                {
                    aheight = awidth * height / (2 * width);
                }
            }
        }

        public void MakeAscii()
        {
            ascii = new char[(awidth+1) * aheight];
            int wres = width / awidth;
            int hres = height / aheight;

            for (int y = 0; y < aheight; y++)
            {
                for(int x = 0; x < awidth; x++)
                {
                    int count = 0;
                    int totalInt = 0;

                    for (int ox = x * wres; ox < (x + 1) * wres; ox++)
                        for (int oy = y * hres; oy < (y + 1) * hres; oy++)
                        {
                            count++;
                            totalInt += gray[ox + oy * width];
                        }

                    int index = 12 * totalInt / (255 * count);
                    ascii[x + y * awidth] = chars[index];
                }
            }
        }

        public byte[] Gray { get; }
        public char[] Ascii { get; set; }
        //public char[] Chars { get; }

        public int width, height, awidth = 10, aheight;
        Bitmap original;
        byte[] gray;
        public char[] ascii;
        public bool invert = false;

        private object locker = new object();

        public float contrastFactor=1;

        //char[] chars = new char[] { ' ', '.', ',', '-', '~', ':', ';', '=', '*', '#', '%', '$', '@' };
        public char[] chars = (new string(" .,-~:;=*#%$@")).ToCharArray();
    }
}
