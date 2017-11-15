using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    
    public class BTree
    {        
        TreeNode root;
        public string myfield;

        public BTree(string field)
        {
            root = new TreeNode();
            myfield = field;
        }

        private void save()
        {

        }

        public void load()
        {

        }

        public void insert(string dato, int index)
        {
            TreeNode current = root;
            while (current != null)
            {
                //el nodo ya existe, se agrega el indice
                int i = current.findValue(dato);
                if (i != -1)
                {
                    //ordenar de ser posible
                    current.values[i].indices.Add(index);
                    return;
                }
                // es hoja
                if (current.sons.Count == 0)
                {
                    current.addValue(new Record(dato,index));                    
                    splitRecursive(current);
                    return;
                }
                //no es hoja
                else                
                    current = current.findNextNode(dato);                                    
            }
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

        public TreeNode findNode(string data)
        {
            TreeNode current = root;
            while (current != null)
            {
                //busca el dato dentro del nodo
                int i = current.findValue(data);
                if (i != -1)
                    return current;
                current = current.findNextNode(data);
            }
            return null;
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
