﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    
    public class BTree
    {        
        TreeNode root;
        public string name;
        public bool primary;

        public BTree(string field)
        {
            root = new TreeNode();
            name = field;
            primary = false;
        }
        public BTree(string field, bool pri)
        {
            root = new TreeNode();
            name = field;
            primary = pri;
        }

        public void save(string tableName)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + tableName + "/" + name + ".index", FileMode.Create));
            bw.Write(primary);
            save(root,bw);
            bw.Close();
        }

        private void save(TreeNode n, BinaryWriter bw)
        {
            bw.Write(n.values.Count);
            for (int i = 0; i < n.values.Count; i++)
            {
                bw.Write(n.values[i].value);
                bw.Write(n.values[i].indices.Count);
                for (int j = 0; j < n.values[i].indices.Count; j++)                
                    bw.Write(n.values[i].indices[j]);                
            }
            bw.Write(n.sons.Count);
            for (int i = 0; i < n.sons.Count; i++)            
                save(n.sons[i], bw);            
        }

        public void load(string tableName)
        {
            BinaryReader br = new BinaryReader(new FileStream("BD/" + tableName + "/" + name + ".index", FileMode.Open));
            primary = br.ReadBoolean();
            load(root, br);
            br.Close();
        }

        private void load(TreeNode n, BinaryReader br)
        {
            int valuesCount = br.ReadInt32();
            for (int i = 0; i < valuesCount; i++)
            {
                n.values.Add(new Record(br.ReadString()));
                int indicesCount = br.ReadInt32();
                for (int j = 0; j < indicesCount; j++)                
                    n.values[i].indices.Add(br.ReadInt32());
            }
            int sonsCount = br.ReadInt32();
            for (int i = 0; i < sonsCount; i++)
            {
                n.sons.Add(new TreeNode());
                load(n.sons[i], br);
            }
                
        }

        public bool insert(string dato, int index)
        {
            TreeNode current = root;
            while (current != null)
            {
                //el nodo ya existe, se agrega el indice
                int i = current.findValue(dato);
                if (i != -1)
                {
                    if (primary && current.values[i].indices.Count > 0)
                        return false;
                    current.values[i].indices.Add(index);
                    return true; ;
                }
                // es hoja
                if (current.sons.Count == 0)
                {
                    current.addValue(new Record(dato,index));                    
                    splitRecursive(current);
                    return true;
                }
                //no es hoja
                else                
                    current = current.findNextNode(dato);                                    
            }
            return false;
        }

        private void splitRecursive(TreeNode current)
        {
            if (current.values.Count <= TreeNode.maximum)
                return;
            //hay overflow
            TreeNode sibling = new TreeNode(); ;
            Record newRecord = current.split(sibling);
            //es el root, se necista crear nodo nuevo
            if (current.father == null)
            {
                root = new TreeNode(newRecord, current, sibling);
                current.father = sibling.father = root;
                return;
            }
            // se añada al padre existente
            current.father.addValue(newRecord, sibling);
            splitRecursive(current.father);
        }

        public void delete(string dato, int index)
        {
            TreeNode current = root;
            while (current != null)
            {
                //el nodo ya existe, se agrega el indice
                int i = current.findValue(dato);
                if (i != -1)
                {
                    current.values[i].indices.Remove(index);
                    return;
                }
                current = current.findNextNode(dato);
            }
        }

        public List<int> findIndices(string data)
        {             
            TreeNode current = root;
            while (current != null)
            {
                //busca el dato dentro del nodo
                int i = current.findValue(data);
                if (i != -1)
                    return current.values[i].indices;
                current = current.findNextNode(data);
            }
            return null;
        }
        
        public bool exist(string data)
        {
            TreeNode current = root;
            while (current != null)
            {
                //busca el dato dentro del nodo
                int i = current.findValue(data);
                if (i != -1 && current.values[i].indices.Count > 0)
                    return true;
                current = current.findNextNode(data);
            }
            return false;
        }

        public void show()
        {
            //Console.WriteLine( root.values[0]);
            show(root," ");
        }

        private void show(TreeNode n, string space)
        {
            if (n == null)
                return;
            Console.Write("{0}", space);
            for (int i = 0; i < n.values.Count; i++)            
                Console.Write("{0} ", n.values[i].value);
            Console.WriteLine();
            for (int i = 0; i < n.sons.Count; i++)
                show(n.sons[i], space + " ");
        }

    }// fin Btree
} // fin namespace
