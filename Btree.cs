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
        public bool memory;
        private int nodesCount;                

        List<int> visitedNodes;
        List<TreeNode> visitedNodesPtr;

        public BTree(string table_, string field_, bool primary_)
        {
            nodes = new List<TreeNode>();
            field = field_;
            table = table_;
            primary = primary_;
            visitedNodes = new List<int>();
            visitedNodesPtr = new List<TreeNode>();
            memory = true;           
            if (File.Exists("BD/" + table + "/" + field + ".btree"))
            {
                loadMemory();
            }
            else
            {
                root = 0;
                nodes.Add(new TreeNode(root));                
                BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Create));
                bw.Write(root);
                bw.Write(primary);
                bw.Write(nodes.Count);
                nodes[0].saveAppend(bw);
                nodesCount = 1;
                bw.Close();
                if (!memory)
                    nodes.Clear();
            }
        }
        public void loadMemory()
        {
            BinaryReader br = new BinaryReader(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Open));
            root = br.ReadInt32();
            primary = br.ReadBoolean();
            nodesCount = br.ReadInt32();
            if (memory)
            {
                long pos = 9;
                while (pos < br.BaseStream.Length)
                {
                    nodes.Add(new TreeNode(br, pos));
                    pos += myfunctions.nodeSize;
                }
            }
            br.Close();
        }
       
        public bool insert(string key, int index)
        {
            visitedNodes.Clear();
            visitedNodesPtr.Clear();
            int current = root;
            TreeNode temp;
            while (current != -1)
            {
                visitedNodes.Add(current);
                if (memory)
                    temp = nodes[current];
                else
                {
                    temp = new TreeNode(current);
                    temp.load(table, field);
                }
                    
                visitedNodesPtr.Add(temp);

                int i = temp.findValue(key);
                if (i != -1)
                {
                    temp.indices[i] = index;
                    saveNodes(0);
                    return true; //modificar
                }
                if (temp.pointers[0] == -1)
                {
                    temp.addValue(key, index);
                    int newNodes = splitNodes();
                    saveNodes(newNodes);
                    return true;
                }
                current = temp.findNextNode(key);
            }
            return false;
        }

        private int splitNodes()
        {            
            bool newNode = false;
            string key = "";
            int index = 0;
            int modified = 0;            

            for (int i = visitedNodesPtr.Count - 1; i >= 0; i--)
            {
                if (newNode)
                {
                    visitedNodesPtr[i].addValue(key, index, nodesCount -1 );
                    newNode = false;
                }

                if (visitedNodesPtr[i].keys.Count <= myfunctions.maxSize)
                    break;

                key = visitedNodesPtr[i].keys[myfunctions.minSize];
                index = visitedNodesPtr[i].indices[myfunctions.minSize];
                TreeNode sibling = new TreeNode(visitedNodesPtr[i], nodesCount);
                if (memory)
                    nodes.Add(sibling);
                visitedNodesPtr.Add(sibling);
                visitedNodes.Add(nodesCount);
                nodesCount++;
                newNode = true;
                modified++;
            }
            if (newNode)
            {
                TreeNode newRoot = new TreeNode(nodesCount);
                newRoot.addValue(key, index);
                newRoot.pointers[0] = root;
                newRoot.pointers[1] = nodesCount-1;
                root = nodesCount;
                if (memory)
                    nodes.Add(newRoot);                
                visitedNodesPtr.Add(newRoot);
                visitedNodes.Add(nodesCount);
                nodesCount++;
                modified++;
            }           
            return modified;
        }

        public void delete(string key, int index)
        {
            int current = root;
            TreeNode temp;
            while (current != -1)
            {
                if (memory)
                    temp = nodes[current];
                else
                {
                    temp = new TreeNode(current);
                    temp.load(table, field);
                }
                int i = temp.findValue(key);
                if (i != -1)
                {
                    temp.indices[i] = -1;
                    visitedNodesPtr.Add(temp);
                    saveNodes(0);
                    return;
                }
                current = temp.findNextNode(key);
            }
        }

        private void saveNodes(int newNodes)
        {
            BinaryWriter bw;
            if (newNodes > 0)
            {
                bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Append));
                for (int i = 0; i < newNodes * myfunctions.nodeSize; i++)
                    bw.Write(false);
                bw.Close();
            }
            else
            {
                bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Open));                
                visitedNodesPtr[visitedNodesPtr.Count - 1].saveOpen(bw);                
                bw.Close();
                return;
            }
            
            bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Open));
            for (int i = 0; i < visitedNodesPtr.Count; i++)            
                visitedNodesPtr[i].saveOpen(bw);
           
            bw.BaseStream.Seek(0,SeekOrigin.Begin);
            bw.Write(root);
            bw.Write(primary);
            bw.Write(nodesCount);
            bw.Close();
        }        

        public List<int> findIndices(string key)
        {
            List<int> indices = new List<int>();           
            int current = root;
            TreeNode temp;
            while (current != -1)
            {
                if (memory)
                    temp = nodes[current];
                else
                {
                    temp = new TreeNode(current);
                    temp.load(table, field);
                }
                int i = temp.findValue(key);                
                if (i != -1)
                {
                    if (temp.indices[i] == -1)
                        return indices;                  
                    indices.Add(temp.indices[i]);              
                    return indices;
                }
                    
                current = temp.findNextNode(key);
            }
            return indices;
        } 
        
        public bool exist(string key)
        {
            int current = root;
            TreeNode temp;
            while (current != -1)
            {
                if (memory)
                    temp = nodes[current];
                else
                {
                    temp = new TreeNode(current);
                    temp.load(table, field);
                }
                int i = temp.findValue(key);
                if (i != -1 && temp.indices[i] != -1)
                    return true;
                current = temp.findNextNode(key);
            }
            return false;
        }

        public void show()
        {
            Console.WriteLine("nodos count: {0}", nodesCount);
            show(root," ");
        }

        private void show(int current, string space)
        {
            if (current == -1)
                return;
            Console.Write("({1}){0}", space, nodes[current].pos);
            for (int i = 0; i < nodes[current].keys.Count; i++)            
                Console.Write("{0} ", nodes[current].keys[i]);
            Console.WriteLine();
            for (int i = 0; i < nodes[current].pointers.Count; i++)
                show(nodes[current].pointers[i], space + " ");
        }
        //solo si esta en memoria
        public void saveAll()
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Create));
            bw.Write(root);
            bw.Write(primary);
            bw.Write(nodes.Count);            
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].saveAppend(bw);
            }
            bw.Close();
        }

    }// fin Btree
} // fin namespace
