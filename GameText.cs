using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace sth1edwv
{
    public class GameText
    {
        private readonly Cartridge _cartridge;
        private readonly int _offset;
        private readonly int _maxLength; // max length of the text in characters
        private readonly Dictionary<byte, char> _map;
        private byte _x;
        private byte _y;
        private string _text;

        public GameText(Cartridge cartridge, int offset, bool isLowerMap)
        {
            _cartridge = cartridge;
            _offset = offset;
            _x = cartridge.Memory[offset++];
            _y = cartridge.Memory[offset++];
            _map = isLowerMap ? LowerChars : UpperChars;
            _text = "";
            int length = 0;
            for (;;)
            {
                var b = cartridge.Memory[offset++];
                if (b == 0xff)
                {
                    // end of string
                    _maxLength = length;
                    break;
                }
                ++length;
                if (_map.TryGetValue(b, out char c))
                {
                    _text += c;
                }
            }
        }

        public string AsSerialized => $"{_x};{_y};{_text}";

        public override string ToString()
        {
            return _text;
        }

        private static readonly Dictionary<byte, char> LowerChars = new()
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

        private static readonly Dictionary<byte, char> UpperChars = new()
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

        public void Deserialize(string input)
        {
            var match = Regex.Match(input, "^(?<x>\\d+);(?<y>\\d+);(?<text>.+)$");
            if (!match.Success)
            {
                throw new Exception("Invalid text entered");
            }

            var x = Convert.ToByte(match.Groups["x"].Value);
            var y = Convert.ToByte(match.Groups["y"].Value);
            // We pad the text to the full width with spaces, as the game does
            var text = match.Groups["text"].Value.PadRight(_maxLength, ' ');
            if (text.Length > _maxLength)
            {
                throw new Exception("Text too long");
            }

            // Now we convert to bytes
            var reverseMap = _map.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var buffer = new MemoryStream();
            buffer.WriteByte(x);
            buffer.WriteByte(y);
            foreach (var c in text)
            {
                if (reverseMap.TryGetValue(c, out var b))
                {
                    buffer.WriteByte(b);
                }
                else
                {
                    throw new Exception($"Invalid text: character \"{c}\" is not allowed");
                }
            }
            buffer.WriteByte(0xff);
            // If we get here then we did not fail, finally we write to our members and the cartridge
            _x = x;
            _y = y;
            _text = text;
            Array.Copy(buffer.GetBuffer(), 0, _cartridge.Memory, _offset, buffer.Length);
        }
    }
}
