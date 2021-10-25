using System;
using System.Collections;

namespace Be.Windows.Forms
{
    internal class DataMap : ICollection, IEnumerable
    {
        readonly object _syncRoot = new object();
        internal int count;
        internal DataBlock firstBlock;
        internal int version;

        public DataMap()
        {
        }

        public DataMap(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (DataBlock item in collection)
            {
                AddLast(item);
            }
        }

        public DataBlock FirstBlock
        {
            get
            {
                return firstBlock;
            }
        }

        public void AddAfter(DataBlock block, DataBlock newBlock)
        {
            AddAfterInternal(block, newBlock);
        }

        public void AddBefore(DataBlock block, DataBlock newBlock)
        {
            AddBeforeInternal(block, newBlock);
        }

        public void AddFirst(DataBlock block)
        {
            if (firstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddBeforeInternal(firstBlock, block);
            }
        }

        public void AddLast(DataBlock block)
        {
            if (firstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddAfterInternal(GetLastBlock(), block);
            }
        }

        public void Remove(DataBlock block)
        {
            RemoveInternal(block);
        }

        public void RemoveFirst()
        {
            if (firstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(firstBlock);
        }

        public void RemoveLast()
        {
            if (firstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(GetLastBlock());
		}

		public DataBlock Replace(DataBlock block, DataBlock newBlock)
		{
			AddAfterInternal(block, newBlock);
			RemoveInternal(block);
			return newBlock;
		}

        public void Clear()
        {
            DataBlock block = FirstBlock;
            while (block != null)
            {
                DataBlock nextBlock = block.NextBlock;
                InvalidateBlock(block);
                block = nextBlock;
            }
            firstBlock = null;
            count = 0;
            version++;
        }

        void AddAfterInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock.previousBlock = block;
            newBlock.nextBlock = block.nextBlock;
            newBlock.map = this;

            if (block.nextBlock != null)
            {
                block.nextBlock.previousBlock = newBlock;
            }
            block.nextBlock = newBlock;

            this.version++;
            this.count++;
        }

        void AddBeforeInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock.nextBlock = block;
            newBlock.previousBlock = block.previousBlock;
            newBlock.map = this;

            if (block.previousBlock != null)
            {
                block.previousBlock.nextBlock = newBlock;
            }
            block.previousBlock = newBlock;

            if (firstBlock == block)
            {
                firstBlock = newBlock;
            }
            this.version++;
            this.count++;
        }

        void RemoveInternal(DataBlock block)
        {
            DataBlock previousBlock = block.previousBlock;
            DataBlock nextBlock = block.nextBlock;

            if (previousBlock != null)
            {
                previousBlock.nextBlock = nextBlock;
            }

            if (nextBlock != null)
            {
                nextBlock.previousBlock = previousBlock;
            }

            if (firstBlock == block)
            {
                firstBlock = nextBlock;
            }

            InvalidateBlock(block);

            count--;
            version++;
        }

        DataBlock GetLastBlock()
        {
            DataBlock lastBlock = null;
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                lastBlock = block;
            }
            return lastBlock;
        }

        void InvalidateBlock(DataBlock block)
        {
            block.map = null;
            block.nextBlock = null;
            block.previousBlock = null;
        }

        void AddBlockToEmptyMap(DataBlock block)
        {
            block.map = this;
            block.nextBlock = null;
            block.previousBlock = null;

            firstBlock = block;
            version++;
            count++;
        }

        #region ICollection Members
        public void CopyTo(Array array, int index)
        {
            DataBlock[] blockArray = array as DataBlock[];
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                blockArray[index++] = block;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }
        #endregion

        #region IEnumerable Members
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        #region Enumerator Nested Type
        internal class Enumerator : IEnumerator, IDisposable
        {
            readonly DataMap _map;
            DataBlock _current;
            int _index;
            readonly int _version;

            internal Enumerator(DataMap map)
            {
                _map = map;
                _version = map.version;
                _current = null;
                _index = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0 || _index > _map.Count)
                    {
                        throw new InvalidOperationException("Enumerator is positioned before the first element or after the last element of the collection.");
                    }
                    return _current;
                }
            }

            public bool MoveNext()
            {
                if (this._version != _map.version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_index >= _map.Count)
                {
                    return false;
                }

                if (++_index == 0)
                {
                    _current = _map.FirstBlock;
                }
                else
                {
                    _current = _current.NextBlock;
                }

                return (_index < _map.Count);
            }

            void IEnumerator.Reset()
            {
                if (this._version != this._map.version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                this._index = -1;
                this._current = null;
            }

            public void Dispose()
            {
            }
        }
        #endregion
    }
}
