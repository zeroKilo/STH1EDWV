using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sth1edwv.GameObjects
{
    public class EnumerableMemoryStream: IEnumerable<byte>
    {
        private readonly MemoryStream _memoryStream;

        public EnumerableMemoryStream(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream;
            memoryStream.Position = 0;
        }

        public class Enumerator : IEnumerator<byte>
        {
            private readonly MemoryStream _memoryStream;

            public Enumerator(MemoryStream memoryStream)
            {
                _memoryStream = memoryStream;
            }

            public void Dispose()
            {
                _memoryStream.Dispose();
            }

            public bool MoveNext()
            {
                var value = _memoryStream.ReadByte();
                if (value == -1)
                {
                    return false;
                }

                Current = (byte)value;
                return true;
            }

            public void Reset()
            {
                _memoryStream.Position = 0;
            }

            public byte Current { get; private set; }

            object IEnumerator.Current => Current;
        }
        public IEnumerator<byte> GetEnumerator()
        {
            return new Enumerator(_memoryStream);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}