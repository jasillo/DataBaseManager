﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    class TableDescriptor
    {
        public List<string> myFields;
        public List<string> myTypes;
        public List<BinaryTree<int>> btreesInts;
        public List<BinaryTree<string>> btreesStrings;
        public string name;
        public List<int> hollows;
        public List<int> indices; //indices de las filas
        public int rowSize;
        private List<List<string>> buffer;
        private int end = 0;

        public TableDescriptor(string tableName)
        {
            name = tableName;
            myFields = new List<string>();
            myTypes = new List<string>();
            hollows = new List<int>();
            indices = new List<int>();
            buffer = new List<List<string>>();  //coumn -row
            btreesInts = new List<BinaryTree<int>>();
            btreesStrings = new List<BinaryTree<string>>();

            rowSize = 0;
        }

        ~TableDescriptor()
        {
            saveLists();
        }

        public bool addField(string fieldName, string typeName)
        {
            for (int i = 0; i < myFields.Count; i++)
            {
                if (myFields[i] == fieldName)
                    return false;
            }
            myFields.Add(fieldName);
            myTypes.Add(typeName);
            if (typeName == "varchar")            
                rowSize += myfunctions.stringSize;                
            else if (typeName == "integer")
                rowSize += myfunctions.intSize;
            else if (typeName == "date")
                rowSize += myfunctions.dateSize;
            else if (typeName == "boolean")
                rowSize += myfunctions.boolSize;            
            return true;
        }

        public string show()
        {
            string r = String.Format("Tabla {0} {1}",name, Environment.NewLine);
            for (int i = 0; i < myFields.Count; i++)
            {
                r += String.Format("\t{0} {1}{2}",myFields[i], myTypes[i], Environment.NewLine);
            }
            r += String.Format("\trow size : {0} {1}", rowSize, Environment.NewLine);
            return r;
        }

        public void insertRow(List<string> values)
        {
            BinaryWriter bw;
            if (hollows.Count == 0)
            {
                bw = new BinaryWriter(File.Open(name + ".table", FileMode.Append));
                for (int i = 0; i < myTypes.Count; i++)
                {
                    if (myTypes[i] == "integer")
                        bw.Write(Int32.Parse(values[i]));
                    else if (myTypes[i] == "boolean")
                        bw.Write(Boolean.Parse(values[i]));
                    else if (myTypes[i] == "varchar")
                        bw.Write(myfunctions.fixedString(values[i]));
                    else
                        bw.Write(values[i]);
                }
                bw.Close();
                indices.Add(end);
                end += rowSize;
            }
            else
            {
                int index = hollows[0];
                hollows.RemoveAt(0);
                indices.Add(index);

                bw = new BinaryWriter(File.Open(name + ".table", FileMode.Open));
                bw.BaseStream.Seek(index, SeekOrigin.Begin);
                for (int i = 0; i < myTypes.Count; i++)
                {
                    if (myTypes[i] == "integer")
                        bw.Write(Int32.Parse(values[i]));
                    else if (myTypes[i] == "boolean")
                        bw.Write(Boolean.Parse(values[i]));
                    else if (myTypes[i] == "varchar")
                        bw.Write(myfunctions.fixedString(values[i]));
                    else
                        bw.Write(values[i]);
                }
                bw.Close();                
            }            
        }

        public string select(List<int> columnIndices)
        {
            string r = "";
            
            List<int> indexFields = new List<int>();
            for (int i = 0; i < buffer.Count; i++)
            {
                for (int j = 0; j < columnIndices.Count; j++)
                {
                    r += String.Format("{0} \t",buffer[i][columnIndices[j]]);
                }
                r += String.Format("{0}", Environment.NewLine);
            }            
            return r;
        }

        //borra el contenido del buffer de la data fisica
        public void deleteBuffer()
        {
            List<int> temp = new List<int>();
            for (int rowIndex = 0; rowIndex < buffer.Count; rowIndex++)
                temp.Add(Int32.Parse(buffer[rowIndex][myFields.Count]));
            
            hollows.AddRange(temp);
            indices = indices.Except(temp).ToList();            
        }

        //graba el contenido del buffer en la data fisica
        public void saveBuffer()
        {

        }

        public void fillBuffer(List<string> where)
        {            
            fill(indices);            
            if (where == null || where.Count == 0)
                return;
            int columnIndex = isField(where[0]);
            //recorrido a traves de la filas
            int type;
            if (where[1] == "=")
                type = 0;
            else if (where[1] == "<")
                type = 1;
            else
                type = 2;

            if (where[3] == "varchar")
            {
                for (int rowIndex = 0; rowIndex < buffer.Count; rowIndex++)
                {
                    if (!compareStrings(buffer[rowIndex][columnIndex], where[2], type))
                    {
                        buffer.RemoveAt(rowIndex);
                        rowIndex--;
                    }
                }
            }
            else if (where[3] == "integer")
            {
                int data2 = Int32.Parse(where[2]);
                for (int rowIndex = 0; rowIndex < buffer.Count; rowIndex++)
                {
                    if (!compareInts(Int32.Parse (buffer[rowIndex][columnIndex]), data2, type))
                    {
                        buffer.RemoveAt(rowIndex);
                        rowIndex--;
                    }
                }
            }
        }

        private bool compareStrings(string data1, string data2, int type)
        {
            if (type == 0)
                return data1 == data2;
            return false;
        }

        private bool compareInts(int data1, int data2, int type)
        {
            if (type == 0)
                return data1 == data2;
            if (type == 1)
                return data1 < data2;
            if (type == 2)
                return data1 > data2;
            return false;
        }

        private bool compareBools(bool data1, bool data2, int type)
        {
            if (type == 0)
                return data1 == data2;
            return false;
        }       

        private void fill(List<int> indices)
        {
            buffer.Clear();
            //creando el numero de filas como indices
            for (int i = 0; i < indices.Count; i++)
                buffer.Add(new List<string>());

            BinaryReader br;
            br = new BinaryReader(File.Open(name + ".table", FileMode.Open));
            
            for (int i = 0; i < indices.Count; i++)
            {
                br.BaseStream.Seek(indices[i], SeekOrigin.Begin);
                for (int j = 0; j < myTypes.Count; j++)
                {
                    if (myTypes[j] == "integer")
                        buffer[i].Add (br.ReadInt32().ToString());
                    else if (myTypes[j] == "boolean")
                        buffer[i].Add(br.ReadBoolean().ToString());
                    else if (myTypes[j] == "varchar")
                        buffer[i].Add(myfunctions.getString(br.ReadString()));
                    else
                        buffer[i].Add(br.ReadString());
                }
                buffer[i].Add(indices[i].ToString());
            }
            br.Close();
            
        }

        public int isField(string field)
        {
            for (int i = 0; i < myFields.Count; i++)
            {
                if (myFields[i] == field)
                    return i;
            }
            return -1;
        }

        public void saveLists()
        {
            BinaryWriter bw;
            bw = new BinaryWriter(new FileStream(name + ".list", FileMode.Create));
            bw.Write(indices.Count);
            for (int i = 0; i < indices.Count; i++)
            {
                bw.Write(indices[i]);
            }
            bw.Write(hollows.Count);
            for (int i = 0; i < hollows.Count; i++)
            {
                bw.Write(hollows[i]);
            }
            bw.Close();
        }

        public void loadLists()
        {
            BinaryReader br;
            br = new BinaryReader(new FileStream(name + ".list", FileMode.Open));
            int tam = br.ReadInt32();
            for (int i = 0; i < tam; i++)
            {
                indices.Add(br.ReadInt32());
            }
            tam = br.ReadInt32();
            for (int i = 0; i < tam; i++)
            {
                hollows.Add(br.ReadInt32());
            }
            br.Close();
        }
        
        public bool findIndex(int fieldIndex, string indexName)
        {
            if (myTypes[fieldIndex] == "varchar")
            {
                for (int i = 0; i < btreesStrings.Count; i++)
                {
                    if (btreesStrings[i].myName == indexName)
                        return true;
                }
            }
            else if(myTypes[fieldIndex] == "integer")
            {
                for (int i = 0; i < btreesInts.Count; i++)
                {
                    if (btreesStrings[i].myName == indexName)
                        return true;
                }
            }
            return false;
        }

        public void createIndex(int fieldIndex, string indexName)
        {
            BinaryWriter bw;
            bw = new BinaryWriter(new FileStream(indexName + ".index", FileMode.Create));
            bw.Close();

            fillBuffer(null);
            if (myTypes[fieldIndex] == "varchar")
            {
                btreesStrings.Add(new BinaryTree<string>(indexName, myFields[fieldIndex]));
                BinaryTree<string> temp = btreesStrings[btreesStrings.Count - 1];
                for (int row = 0; row < buffer.Count; row++)
                {
                    string dato = buffer[row][fieldIndex];
                    int index = Int32.Parse(buffer[row][myFields.Count]);
                    temp.insert(dato, index);
                }
            }
            else if (myTypes[fieldIndex] == "integer")
            {
                btreesInts.Add(new BinaryTree<int>(indexName, myFields[fieldIndex]));
                BinaryTree<int> temp = btreesInts[btreesInts.Count - 1];
                for (int row = 0; row < buffer.Count; row++)
                {
                    int dato = Int32.Parse(buffer[row][fieldIndex]);
                    int index = Int32.Parse(buffer[row][myFields.Count]);
                    temp.insert(dato, index);
                }
            }
        }

        public void saveIndices()
        {
            BinaryWriter bw;

            for (int i = 0; i <btreesStrings.Count; i++)
            {

            }
            for ()
            bw = new BinaryWriter(new FileStream(name + ".list", FileMode.Create));
            bw.Write(indices.Count);
            for (int i = 0; i < indices.Count; i++)
            {
                bw.Write(indices[i]);
            }
            bw.Write(hollows.Count);
            for (int i = 0; i < hollows.Count; i++)
            {
                bw.Write(hollows[i]);
            }
            bw.Close();
        }
    }
}
