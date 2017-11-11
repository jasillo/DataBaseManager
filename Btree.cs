using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    

    public class TreeNode<T>
    {
        public T myData;
        public TreeNode<T> left;
        public TreeNode<T> right;
        public List<int> indices;

        public TreeNode(T data)
        {
            myData = data;
            left = null;
            right = null;
            indices = new List<int>();
        }
    }    

    public class BinaryTree<T>
    {        
        TreeNode<T> root;
        public string myName;
        string myfield;

        public BinaryTree(string name, string field)
        {
            myName = name;
            root = null;
            myfield = field;
        }

        private void save()
        {

        }

        public void load()
        {

        }

        public List<int> findIndices(T data)
        {
            TreeNode<T> temp = findNode(root, data);
            if (temp == null)
                return null;
            return temp.indices;
        }

        public void insert(T dato, int index)
        {
            if (root == null)
            {
                root = new TreeNode<T>(dato);
                return;
            }
            TreeNode<T> temp = findNode(root, dato);
            //no existe el nodo
            if (temp == null)
            {
                temp = findFather(root, dato);
                if (compare(dato, temp.myData))
                {
                    temp.left = new TreeNode<T>(dato);
                    temp.left.indices.Add(index);
                }
                else
                {
                    temp.right = new TreeNode<T>(dato);
                    temp.right.indices.Add(index);
                }                    
                return;
            }
            //ya existe el nodo
            temp.indices.Add(index);
        }

        public bool delete(T dato, int index)
        {
            TreeNode<T> temp;
            temp = findNode(root, dato);
            if (temp == null)
                return false;
            for (int i = 0; i < temp.indices.Count; i++)
            {
                if (temp.indices[i] == index)
                {
                    temp.indices.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        TreeNode<T> findFather(TreeNode<T> n, T data)
        {
            if (n == null)
                return n;
            if (data.Equals(n.myData))
                return n;
            TreeNode<T> temp;
            if (compare(data, n.myData))
                temp = findFather(n.left, data);
            else
                temp = findFather(n.right, data);

            if (temp == null)
                return n;
            return temp;           
        }

        TreeNode<T> findNode(TreeNode<T> n, T data)
        {
            if (n == null)
                return n;
            if (data.Equals(n.myData))
                return n;
            TreeNode<T> temp;
            if (compare(data, n.myData))
                temp = findNode(n.left, data);
            else
                temp = findNode(n.right, data);
            return temp;
        }

        bool compare(T a, T b)
        {
            if (typeof(T) == typeof(int))
                return Convert.ToInt32(a) < Convert.ToInt32(b);
            if (typeof(T) == typeof(string))
            {
                string A = Convert.ToString(a);
                string B = Convert.ToString(b);
                if (String.Compare(A, B) == -1)
                    return true;
                return false;
            }
            return false;
        }
        public void show()
        {
            show(root,"");
        }
        private void show(TreeNode<T> n, string space)
        {
            if (n == null)
                return;
            Console.WriteLine("{0}{1}",space, n.myData);
            show(n.left, space + " ");
            show(n.right, space + " ");
        }

    }
}
