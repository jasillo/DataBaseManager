using System;
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
        public List<BTree> btrees;
        public string name;
        public List<int> hollows;
        public List<int> indices; //indices de las filas
        public int rowSize;
        private List<List<string>> buffer;
        public int end = 0;

        public TableDescriptor(string tableName)
        {
            name = tableName;
            myFields = new List<string>();
            myTypes = new List<string>();
            hollows = new List<int>();
            indices = new List<int>();
            buffer = new List<List<string>>();  //coumn -row
            btrees = new List<BTree>();

            rowSize = 0;
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

        public bool insertRow(List<string> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                int btreeIndex = findBtreeIndex(myFields[i]);
                if (btreeIndex > -1 && btrees[btreeIndex].primary && btrees[btreeIndex].exist(values[i]))
                    return false;
            }

            BinaryWriter bw;
            if (hollows.Count == 0)
            {

                bw = new BinaryWriter(File.Open("BD/"+ name + "/" + name + ".table", FileMode.Append));
                for (int i = 0; i < myTypes.Count; i++)
                {
                    int btreeIndex = findBtreeIndex(myFields[i]);
                    if (btreeIndex > -1)
                        btrees[btreeIndex].insert(values[i], end);
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
                    int btreeIndex = findBtreeIndex(myFields[i]);
                    if (btreeIndex > -1)
                        btrees[btreeIndex].insert(values[i], end);
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
            return true;           
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
            if (temp.Count == 0)
                return; 
            hollows.AddRange(temp);
            indices = indices.Except(temp).ToList();
                        
            for (int column = 0; column < myFields.Count; column++)
            {
                int btreeIndex = findBtreeIndex(myFields[column]);
                if (btreeIndex == -1)
                    continue;
                for (int row = 0; row < temp.Count; row++)
                    btrees[btreeIndex].delete(buffer[row][column], temp[row]);                
            }                    
        }

        //graba el contenido del buffer en la data fisica
        public void saveBuffer()
        {

        }

        public void save()
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + name +"/" + name + ".list", FileMode.Create));
            bw.Write(indices.Count);
            for (int i = 0; i < indices.Count; i++)            
                bw.Write(indices[i]);            
            bw.Write(hollows.Count);
            for (int i = 0; i < hollows.Count; i++)
                bw.Write(hollows[i]);                        
            bw.Close();

            for (int btreeIndex = 0; btreeIndex < btrees.Count; btreeIndex++)            
                btrees[btreeIndex].save(name);           
        }

        public void load()
        {
            BinaryReader br = new BinaryReader(new FileStream("BD/" + name + "/" + name + ".list", FileMode.Open));
            int indicesCount = br.ReadInt32();
            for (int i = 0; i < indicesCount; i++)
                indices.Add(br.ReadInt32());
            int hollowsCount = br.ReadInt32();
            for (int i = 0; i < hollowsCount; i++)
                hollows.Add(br.ReadInt32());
            br.Close();

            for (int btreeIndex = 0; btreeIndex < btrees.Count; btreeIndex++)
                btrees[btreeIndex].load(name);
        }

        public void fillBuffer(List<string> where)
        {            
            if (where == null || where.Count == 0)
            {
                fill(indices);
                return;
            }
                
            int index = findFieldIndex(where[0]);
            //no existe indice para ese campo
            if (index == -1)
            {
                fill(indices);
                if (where == null || where.Count == 0)
                    return;
                int column = isField(where[0]);
                for (int row = 0; row < buffer.Count; row++)
                {                    
                    if (String.Compare(buffer[row][column], where[2]) != 0)
                    {
                        buffer.RemoveAt(row);
                        row--;
                    }
                }
                return;
            }
            //exite indice para el campo
            List<int> indicesTree = btrees[index].findIndices(where[2]);
            fill(indicesTree);
        }      

        private void fill(List<int> indices)
        {
            buffer.Clear();
            //creando el numero de filas como indices
            for (int i = 0; i < indices.Count; i++)
                buffer.Add(new List<string>());

            BinaryReader br;
            br = new BinaryReader(File.Open("BD/"+ name + "/" + name + ".table", FileMode.Open));
            
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
                
        public int findFieldIndex(string fieldName)
        {
            for (int i = 0; i < btrees.Count; i++)
            {
                if (btrees[i].name == fieldName)
                    return i;
            }            
            return -1;
        }

        public bool createIndex(string fieldName, bool isprimary)
        {   
            fillBuffer(null);
            
            int column = isField(fieldName);
            BTree temp = new BTree(fieldName, isprimary);            

            for (int row = 0; row < buffer.Count; row++)
            {
                string dato = buffer[row][column]; //campo
                int index = Int32.Parse(buffer[row][myFields.Count]); //posicion
                if (!temp.insert(dato, index))
                    return false;
            }
            btrees.Add(temp);
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + name + "/" + fieldName + ".index", FileMode.Create));
            bw.Close();
            return true;                        
        }        

        public bool dropIndex(string fieldName)
        {
            int btreeIndex = findBtreeIndex(fieldName);
            if (btreeIndex == -1)
                return false;
            btrees.RemoveAt(btreeIndex);
            if (File.Exists("BD/" + name + "/" + fieldName + ".index"))
                File.Delete("BD/" + name + "/" + fieldName + ".index");
            return true;
        }

        private int findBtreeIndex(string field)
        {
            for (int i = 0; i < btrees.Count; i++)
            {
                if (String.Compare(btrees[i].name, field) == 0)
                    return i;
            }
            return -1;
        }
    }
}
