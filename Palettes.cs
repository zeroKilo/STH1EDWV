using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace sth1edwv
{
    public static class Palettes
    {
        public static List<Color[]> palettes;
        public static void ReadPallettes(byte[] mem)
        {
            palettes = new List<Color[]>();
            int start = 0x627C;
            for (int i = 0; i < 8; i++)
            {
                ushort address = BitConverter.ToUInt16(mem, start);
                Color[] pal = new Color[32];
                for (int j = 0; j < 32; j++)
                {
                    byte col = mem[address + j];
                    byte r = scaleColor((byte)(col & 3));
                    byte g = scaleColor((byte)((col >> 2) & 3));
                    byte b = scaleColor((byte)((col >> 4) & 3));
                    pal[j] = Color.FromArgb(r, g, b);
                }
                palettes.Add(pal);
                start += 2;
            }
        }

        public static byte scaleColor(byte c)
        {
            switch (c)
            {
                case 0:
                    return 0;
                case 1:
                    return 80;
                case 2:
                    return 175;
                case 3:
                    return 255;
            }
            return 0;
        }
    }
}
