using System;
using System.IO;
using System.Collections.Generic;

namespace DataBaseManager
{ 
    public class TreeNode
    {
        public List<int> pointers;
        public List<int> indices;
        public List<string> keys;
        public int pos;    

        public TreeNode(int pos_)
        {
            pointers = new List<int>();
            indices = new List<int>();
            keys = new List<string>();
            pointers.Add(-1);
            pos = pos_;
        }       

        //copia la mitad del nodo
        public TreeNode(TreeNode sibling, int pos_)
        {
            pointers = new List<int>();
            indices = new List<int>();
            keys = new List<string>();
            pos = pos_;

            keys.AddRange(sibling.keys.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize));
            sibling.keys.RemoveRange(myfunctions.minSize, myfunctions.maxSize - myfunctions.minSize + 1);

            indices.AddRange(sibling.indices.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize));
            sibling.indices.RemoveRange(myfunctions.minSize, myfunctions.maxSize - myfunctions.minSize + 1);

            pointers.AddRange(sibling.pointers.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize + 1));
            sibling.pointers.RemoveRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize + 1);
        }

        public TreeNode(BinaryReader br, long offset)
        {
            br.BaseStream.Seek(offset,SeekOrigin.Begin);
            pointers = new List<int>();
            indices = new List<int>();
            keys = new List<string>();
            pos = br.ReadInt32();            
            short tam = br.ReadInt16();
            for (int i = 0; i < tam; i++)
                keys.Add(br.ReadString());
            for (int i = 0; i < tam; i++)
                indices.Add(br.ReadInt32());
            for (int i = 0; i < tam+1; i++)
                pointers.Add(br.ReadInt32());
        }

        public void addValue( string key, int index)
        {
            int i = 0;
            for (i = 0; i < keys.Count && String.Compare(key, keys[i]) != -1; i++) {}
            keys.Insert(i, key);
            indices.Insert(i, index);
            pointers.Add(-1);
        }

        public void addValue(string key, int index, int newSon)
        {

            int i = 0;
            for (i = 0; i < keys.Count && String.Compare(key, keys[i]) != -1; i++) {}            
            keys.Insert(i, key);
            indices.Insert(i, index);
            pointers.Insert(i + 1, newSon);    
        }        

        public int findValue(string key)
        {
            for (int i = 0; i < keys.Count; i++)
            {                
                if (String.Compare(key, keys[i]) == 0)
                    return i;
            }            
            return -1;
        }

        public int findNextNode(string key)
        {
            int i;
            for (i = 0; i < keys.Count && String.Compare(key, keys[i]) != -1; i++) {}
            return pointers[i];
        }  
        
        public void saveAppend (BinaryWriter bw)
        {
            long initialPos = bw.BaseStream.Position;
            bw.Write(pos);
            short tam = (short)keys.Count;
            bw.Write(tam);
            for (int i = 0; i < keys.Count; i++)
                bw.Write(keys[i]);
            for (int i = 0; i < indices.Count; i++)
                bw.Write(indices[i]);
            for (int i = 0; i < pointers.Count; i++)
                bw.Write(pointers[i]);
            long diference = bw.BaseStream.Position - initialPos;
            for (int i = 0; i < myfunctions.nodeSize - diference; i++)
                bw.Write(false);
        }
        
        public void saveOpen (BinaryWriter bw)
        {
            long offset = 9 + pos * myfunctions.nodeSize;
            bw.BaseStream.Seek(offset, SeekOrigin.Begin);
            bw.Write(pos);
            short tam = (short)keys.Count;
            bw.Write(tam);
            for (int i = 0; i < keys.Count; i++)
                bw.Write(keys[i]);
            for (int i = 0; i < indices.Count; i++)
                bw.Write(indices[i]);
            for (int i = 0; i < pointers.Count; i++)
                bw.Write(pointers[i]);
        }   

        public void load(string table, string field)
        {
            BinaryReader br = new BinaryReader(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Open));
            long offset = 9 + pos * myfunctions.nodeSize;
            br.BaseStream.Seek(offset, SeekOrigin.Begin);            
            pos = br.ReadInt32();
            short tam = br.ReadInt16();
            for (int i = 0; i < tam; i++)
                keys.Add(br.ReadString());
            for (int i = 0; i < tam; i++)
                indices.Add(br.ReadInt32());
            pointers.Clear();
            for (int i = 0; i < tam + 1; i++)
                pointers.Add(br.ReadInt32());
            br.Close();
        }
    }
}
