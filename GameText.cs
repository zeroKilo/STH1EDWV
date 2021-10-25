using System.Collections.Generic;
using System.Linq;

namespace sth1edwv
{
    public class GameText
    {
        public bool isLower;
        public byte[] raw;
        public byte x;
        public byte y;
        public string text;
        
        public GameText(byte[] data, bool isLowerMap)
        {
            raw = data;
            isLower = isLowerMap;
            x = data[0];
            y = data[1];
            text = "";
            for (int i = 2; i < data.Length - 1; i++)
            {
                if (isLower && lowChars.ContainsKey(data[i]))
                    text += lowChars[data[i]];
                if (!isLower && highChars.ContainsKey(data[i]))
                    text += highChars[data[i]];
            }
        }

        public void WriteToMemory(Cartridge cartridge, int pos, byte x, byte y, string s, int fillSize = 12)
        {
            cartridge.Memory[pos] = x;
            cartridge.Memory[pos + 1] = y;
            for (int i = 0; i < fillSize; i++)
            {
                if (i < s.Length)
                {
                    if (isLower)
                    {
                        foreach (var pair in lowChars.Where(pair => pair.Value == s[i]))
                        {
                            cartridge.Memory[pos + 2 + i] = pair.Key;
                            break;
                        }
                    }
                    else
                    {
                        foreach (var pair in highChars.Where(pair => pair.Value == s[i]))
                        {
                            cartridge.Memory[pos + 2 + i] = pair.Key;
                            break;
                        }
                    }
                }
                else
                {
                    cartridge.Memory[pos + 2 + i] = 0xEB;
                }
            }
        }

        public static Dictionary<byte, char> lowChars = new Dictionary<byte, char>()
        {
            {0x34, 'A'},	
            {0x35, 'B'},	
            {0x36, 'C'},	
            {0x37, 'D'},	
            {0x44, 'E'},	
            {0x45, 'F'},	
            {0x46, 'G'},	
            {0x47, 'H'},	
            {0x40, 'I'},	
            {0x41, 'J'},	
            {0x42, 'K'},	
            {0x43, 'L'},	
            {0x50, 'M'},	
            {0x51, 'N'},	
            {0x52, 'O'},	
            {0x60, 'P'},	
            {0x61, 'Q'},	
            {0x62, 'R'},	
            {0x70, 'S'},	
            {0x80, 'T'},	
            {0x81, 'U'},	
            {0x54, 'V'},	
            {0x3C, 'W'},	
            {0x3D, 'X'},	
            {0x3E, 'Y'},	
            {0x3F, 'Z'},	
            {0xCF, '©'},	
            {0xEB, ' '}
        };

        public static Dictionary<byte, char> highChars = new Dictionary<byte, char>()
        {
            {0x1E, 'A'},	
            {0x1F, 'B'},	
            {0x2E, 'C'},	
            {0x2F, 'D'},	
            {0x3E, 'E'},	
            {0x3F, 'F'},	
            {0x4E, 'G'},	
            {0x4F, 'H'},	
            {0x5E, 'I'},	
            {0x5F, 'J'},	
            {0x6E, 'K'},	
            {0x6F, 'L'},	
            {0x7E, 'M'},	
            {0x7F, 'N'},	
            {0x8E, 'O'},	
            {0x8F, 'P'},	
            {0x9E, 'Q'},	
            {0x9F, 'R'},	
            {0xAE, 'S'},	
            {0xAF, 'T'},	
            {0xBE, 'U'},	
            {0xBF, 'V'},	
            {0xCE, 'W'},	
            {0xCF, 'X'},	
            {0xDE, 'Y'},	
            {0xDF, 'Z'},	
            {0xAB, '©'},	
            {0xEB, ' '}
        };
    }
}
