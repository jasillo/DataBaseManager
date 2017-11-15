﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class TreeNode
    {
        public List<TreeNode> sons;
        public List<Record> values;
        public TreeNode father;

        public static int minimum = 2;
        public static int maximum = 4;

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
            for (i = 0; i < values.Count && String.Compare(record.value, values[i].value) != -1; i++) { }
            values.Insert(i, record);
            sons.Insert(i+1, newSon);
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
            return r;
        }

        public int findValue(string data)
        {
            for (int i = 0; i < values.Count; i++)
            {
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
    }
}
