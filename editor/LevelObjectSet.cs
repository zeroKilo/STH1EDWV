using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public class LevelObjectSet: IEnumerable<LevelObject>
    {
        // We thinly wrap a list
        public IEnumerator<LevelObject> GetEnumerator() => _objects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _objects.GetEnumerator();
        public LevelObject this[int index] => _objects[index];

        private readonly List<LevelObject> _objects;

        public LevelObjectSet(Cartridge cartridge, int offset)
        {
            var count = cartridge.Memory[offset];
            _objects = Enumerable.Range(0, count)
                .Select(i => new LevelObject(cartridge.Memory, offset + i * 3 + 1))
                .ToList();
        }

        public TreeNode ToNode()
        {
            var result = new TreeNode("Objects");
            result.Nodes.AddRange(_objects.Select(x => x.ToNode()).ToArray());
            return result;
        }
    }
}
