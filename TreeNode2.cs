using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    class TreeNode2
    {
        public List<string> keys;
        public List<long> pointers;
        public List<int> indices;
        public long pos;

        TreeNode2()
        {
            keys = new List<string>();
            pointers = new List<long>();
            indices = new List<int>();
        }

        TreeNode2(long pos_, BinaryReader br)
        {
            keys = new List<string>();
            pointers = new List<long>();
            indices = new List<int>();
            pos = pos_;

            br.BaseStream.Seek(pos, SeekOrigin.Begin);
            short tam = br.ReadInt16();
            for (int i = 0; i < tam; i++)
                keys.Add(br.ReadString());
            for (int i = 0; i < tam; i++)
                indices.Add(br.ReadInt32());
            for (int i = 0; i < tam + 1; i++)
                pointers.Add(br.ReadInt64());
        }

        void save(BinaryWriter bw)
        {
            bw.BaseStream.Seek(pos, SeekOrigin.Begin);
            short tam = (short)keys.Count;
            bw.Write(tam);
            for (int i = 0; i < keys.Count; i++)
                bw.Write(keys[i]);
            for (int i = 0; i < indices.Count; i++)
                bw.Write(indices[i]);
            for (int i = 0; i < pointers.Count; i++)
                bw.Write(pointers[i]);
            long p = bw.BaseStream.Position - pos;
            bool r = false;
            for (int i = 0; i < myfunctions.nodeSize - p; i++)
                bw.Write(r);
        }

        public long insert(string key, int index)
        {
            int i, comp;
            for (i = 0; i < keys.Count; i++)
            {
                comp = String.Compare(key, keys[i]);
                if (comp == 0)
                {
                    indices[i] = index;
                    return -1;
                }
                else if (comp == -1)
                {
                    if (pointers[i] == -1)
                    {
                        keys.Insert(i, key);
                        indices.Insert(i, index);
                        return -1;
                    }
                    return pointers[i];
                }
            }
            if (pointers[i] == -1)
            {
                keys.Insert(i, key);
                indices.Insert(i, index);
                return -1;
            }
            return pointers[i];
        }
    }
}
