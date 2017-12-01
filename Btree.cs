using System;
using System.IO;
using System.Collections.Generic;

namespace DataBaseManager
{
    
    public class BTree
    {
        public List<TreeNode> nodes;
        public string field;
        public string table;
        public bool primary;
        int root; // indice en la lista

        List<int> visitedNodes;

        public BTree(string table_, string field_, bool primary_)
        {
            nodes = new List<TreeNode>();
            field = field_;
            table = table_;
            primary = primary_;
            visitedNodes = new List<int>();
            
            if (File.Exists("BD/" + table + "/" + field + ".btree"))
            {

            }
            else
            {
                nodes.Add(new TreeNode());
                root = 0;
                save();
            }
        }

        public void save()
        {
            Console.WriteLine("BD/" + table + "/" + field + ".btree");
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Create));
            bw.Write(root);
            for (int i = 0; i < nodes.Count; i++)
            {
                
            }
            bw.Close();
        }        

        public void load(string tableName)
        {
            BinaryReader br = new BinaryReader(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Open));
            
            br.Close();
        }
       
        public bool insert(string key, int index)
        {            
            int current = root;            
            while (current != -1)
            {
                visitedNodes.Add(current);
                //el nodo ya existe, se agrega el indice
                int i = nodes[current].findValue(key);
                if (i != -1)
                {
                    nodes[current].indices[i] = index;
                    return true; ;
                }
                // es hoja
                if (nodes[current].pointers[0] == -1)
                {
                    nodes[current].addValue(key,index);
                    splitNodes();
                    visitedNodes.Clear();
                    return true;
                }
                current = nodes[current].findNextNode(key);                                                    
            }
            return false;
        }

        private void splitNodes()
        {
            int current;
            bool newNode = false;
            string key = "";
            int index = 0;
            for (int i = visitedNodes.Count - 1; i >=0; i--)
            {
                current = visitedNodes[i];
                if (newNode)
                {
                    nodes[current].addValue(key, index, nodes.Count - 1);
                    newNode = false;
                }                    
                                
                if (nodes[current].keys.Count <= myfunctions.maxSize)
                    return;
                key = nodes[current].keys[myfunctions.minSize];
                index = nodes[current].indices[myfunctions.minSize];
                TreeNode sibling = new TreeNode(nodes[current]);
                nodes.Add(sibling);
                newNode = true;
            }            
            if (newNode)
            {
                TreeNode newRoot = new TreeNode();
                newRoot.addValue(key, index);
                newRoot.pointers[0] = root;
                newRoot.pointers[1] = nodes.Count - 1;
                root = nodes.Count;
                nodes.Add(newRoot);
            }           
        }

        public void delete(string dato, int index)
        {/*
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
            }*/
        }

        public List<int> findIndices(string key)
        {
            List<int> indices = new List<int>();           
            int current = root;
            while (current != -1)
            {
                int i = nodes[current].findValue(key);
                Console.WriteLine(i);
                if (i != -1)
                {                    
                    indices.Add(nodes[current].indices[i]);              
                    return indices;
                }
                    
                current = nodes[current].findNextNode(key);
            }
            return indices;
        } 
        
        public bool exist(string key)
        {
            int current = root;
            while (current != -1)
            {                
                int i = nodes[current].findValue(key);
                if (i != -1 && nodes[current].indices[i] != -1)
                    return true;
                current = nodes[current].findNextNode(key);
            }
            return false;
        }

        public void show()
        {
            show(root," ");
        }

        private void show(int current, string space)
        {
            if (current == -1)
                return;
            Console.Write("{0}", space);
            for (int i = 0; i < nodes[current].keys.Count; i++)            
                Console.Write("{0} ", nodes[current].keys[i]);
            Console.WriteLine();
            for (int i = 0; i < nodes[current].pointers.Count; i++)
                show(nodes[current].pointers[i], space + " ");
        }

    }// fin Btree
} // fin namespace
