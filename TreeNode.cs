using System;
using System.Collections.Generic;

namespace DataBaseManager
{
    public class Record
    {
        public string value;
        public List<int> indices;

        public Record(string value_, int index)
        {
            value = value_;
            indices = new List<int>();
            indices.Add(index);
        }

        public Record(string value_)
        {
            value = value_;
            indices = new List<int>();
        }

        public int getSize()
        {
            return value.Length + 5 + (indices.Count * 4) ;
        }
    }

    public class TreeNode
    {
        public List<TreeNode> sons;
        public List<Record> values;
        public TreeNode father;
        public long size;

        public static int minimum = 10;
        public static int maximum = 20;

        public TreeNode()
        {
            sons = new List<TreeNode>();
            values = new List<Record>();
        }

        public TreeNode(string value, int index)
        {
            sons = new List<TreeNode>();
            values = new List<Record>();
            values.Add(new Record(value, index));
        }

        //copia la mitad del nodo
        public TreeNode(Record another,TreeNode son1, TreeNode son2)
        {
            sons = new List<TreeNode>();
            values = new List<Record>();
            father = null;
            values.Add(another);
            sons.Add(son1);
            sons.Add(son2);
        }

        public void addValue(Record record)
        {
            int i = 0;
            for (i = 0; i < values.Count && String.Compare(record.value, values[i].value) != -1; i++) { }
            values.Insert(i, record);            
        }

        public void addValue(Record record, TreeNode newSon)
        {

            int i = 0;
            for (i = 0; i < values.Count && String.Compare(record.value, values[i].value) != -1; i++)
            {}            
            values.Insert(i, record);

            for (i = 0; i < sons.Count && String.Compare(newSon.values[0].value, sons[i].values[0].value) != -1; i++)
            {}
            sons.Insert(i, newSon);            
        }

        public Record split(TreeNode sibling)
        {
            sibling.father = father;
            Record r = values[minimum];
            sibling.values.AddRange(values.GetRange(minimum + 1, maximum - minimum));
            values.RemoveRange(minimum, maximum - minimum + 1);
            if (sons.Count == 0)
                return r;
            sibling.sons.AddRange(sons.GetRange(minimum + 1, maximum - minimum + 1));
            sons.RemoveRange(minimum + 1, maximum - minimum + 1);
            for (int i = 0; i < sibling.sons.Count; i++)
                sibling.sons[i].father = sibling;
            return r;
        }

        public int findValue(string data)
        {
            for (int i = 0; i < values.Count; i++)
            {
                //Console.WriteLine("{0} - {1}", data, values[i].value);
                if (String.Compare(data, values[i].value) == 0)
                    return i;
            }            
            return -1;
        }

        public TreeNode findNextNode(string data)
        {
            if (sons.Count == 0)
                return null;
            for (int i = 0; i < values.Count; i++)
            {
                if (String.Compare(data, values[i].value) == -1)
                    return sons[i];
            }
            return sons[sons.Count - 1];
        }

        public int findNext(string data)
        {
            int i;
            for (i = 0; i < values.Count && String.Compare(data, values[i].value ) != -1; i++) {}
            return i;
        }

        public void calculateSize()
        {
            size = 16; //puntero long + int tamanio values + int tamanio sons
            for (int i = 0; i < values.Count; i++)
            {
                size += values[i].getSize(); 
            }
        }
    }
}
