namespace Be.Windows.Forms
{
    internal abstract class DataBlock
    {
        internal DataMap map;
        internal DataBlock nextBlock;
        internal DataBlock previousBlock;

        public abstract long Length
        {
            get;
        }

        public DataMap Map
        {
            get
            {
                return map;
            }
        }

        public DataBlock NextBlock
        {
            get
            {
                return nextBlock;
            }
        }

        public DataBlock PreviousBlock
        {
            get
            {
                return previousBlock;
            }
        }

        public abstract void RemoveBytes(long position, long count);
    }
}
