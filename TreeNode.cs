using System;
using System.Collections.Generic;

namespace DataBaseManager
{ 
    public class TreeNode
    {
        public List<int> pointers;
        public List<int> indices;
        public List<string> keys;
        public long size;

        public TreeNode()
        {
            pointers = new List<int>();
            indices = new List<int>();
            keys = new List<string>();
            pointers.Add(-1);
        }       

        //copia la mitad del nodo
        public TreeNode(TreeNode sibling)
        {
            pointers = new List<int>();
            indices = new List<int>();
            keys = new List<string>();

            keys.AddRange(sibling.keys.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize));
            sibling.keys.RemoveRange(myfunctions.minSize, myfunctions.maxSize - myfunctions.minSize + 1);

            indices.AddRange(sibling.indices.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize));
            sibling.indices.RemoveRange(myfunctions.minSize, myfunctions.maxSize - myfunctions.minSize + 1);

            pointers.AddRange(sibling.pointers.GetRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize + 1));
            sibling.pointers.RemoveRange(myfunctions.minSize + 1, myfunctions.maxSize - myfunctions.minSize + 1);
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
    }
}
