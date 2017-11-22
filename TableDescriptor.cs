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
        public List<int> tempIndices;
        public List<int> tempOffsets;
        public int curretnPosition;
        public int end = 0;
        public BinaryWriter tempbw;
        private bool all;

        public TableDescriptor(string tableName, List<List<string>> b)
        {
            name = tableName;
            myFields = new List<string>();
            myTypes = new List<string>();
            hollows = new List<int>();
            indices = new List<int>();           
            btrees = new List<BTree>();
            tempIndices = new List<int>();
            tempOffsets = new List<int>();
            rowSize = 0;
            curretnPosition = 0;
            buffer = b;
            all = true;
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
            //ver si se viola indice primario
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
                end++;
            }
            else
            {
                int index = hollows[0];
                hollows.RemoveAt(0);
                indices.Add(index);

                bw = new BinaryWriter(File.Open("BD/" + name + "/" + name + ".table", FileMode.Open));
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

        public string select()
        {
            string res = String.Format("numero de filas devueltas : {0}{1}", tempIndices.Count, Environment.NewLine);
            if (buffer.Count == 0)
                return res;
            
            int columnSize = buffer[0].Count;
            for (int r = 0; r < buffer.Count && r < 200; r++)
            {
                for (int c = 0; c < columnSize; c++)
                {
                    res += String.Format("{0} \t",buffer[r][c]);
                }
                res += String.Format("{0}", Environment.NewLine);
            }            
            return res;
        }

        //borra el contenido del buffer de la data fisica
        public void deleteBuffer()
        {
            if (tempIndices.Count == 0)
                return;
            hollows.AddRange(tempIndices);
            indices = indices.Except(tempIndices).ToList();
            //borrar indices del arbol
            int totalRow = 0;
            while (totalRow< tempIndices.Count)
            {
                for (int column = 0; column < myFields.Count; column++)
                {
                    int btreeIndex = findBtreeIndex(myFields[column]);
                    if (btreeIndex > -1)
                    {
                        for (int row = totalRow; row < buffer.Count && row < totalRow + 200; row++)
                        {
                            int i = row - totalRow;
                            btrees[btreeIndex].delete(buffer[i][column], tempIndices[row]);
                        } 
                    }
                }
                totalRow += 200;
                fill(totalRow, totalRow + 200);
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

        public void fillBuffer(List<string> where, List<string> fieldsSelected)
        {
            tempIndices.Clear();
            tempOffsets.Clear();           
            //calculando indices de las filas
            if (where == null || where.Count == 0)
                tempIndices.AddRange(indices);            
            else
            {
                int index = findBtreeIndex(where[0]);
                if (index == -1)
                    getTableScandingIndices(where);
                else
                    tempIndices.AddRange(btrees[index].findIndices(where[2]));                
            }
            //rellenando buffer por campos seleccionados
            if (fieldsSelected[0] == "*")
            {
                fill(0, 200);
                all = true;
                return;
            }

            List<int> fieldsIndices = new List<int>();
            for (int i = 0; i < fieldsSelected.Count; i++)
            {
                tempOffsets.Add(findFieldOffset(fieldsSelected[i]));
                fieldsIndices.Add(isField(fieldsSelected[i]));
            }

            for (int i = 0; i < fieldsSelected.Count; i++)
                fill(tempOffsets, fieldsIndices, 0, 200);
            all = false;
        }      

        private void fill(int ini, int fin)
        {
            buffer.Clear();
            if (tempIndices == null || tempIndices.Count == 0)
                return;
            Console.WriteLine("llego");
            //creando el numero de filas como indices
            for (int i = ini; i < tempIndices.Count && i < fin; i++)
                buffer.Add(new List<string>());

            BinaryReader br;
            br = new BinaryReader(File.Open("BD/"+ name + "/" + name + ".table", FileMode.Open));

            for (int i = ini; i < buffer.Count && i < fin; i++)
            {
                br.BaseStream.Seek(tempIndices[i] * rowSize, SeekOrigin.Begin);
                int row = i - ini;
                for (int j = 0; j < myTypes.Count; j++)
                {
                    if (myTypes[j] == "integer")
                        buffer[row].Add (br.ReadInt32().ToString());
                    else if (myTypes[j] == "boolean")
                        buffer[row].Add(br.ReadBoolean().ToString());
                    else if (myTypes[j] == "varchar")
                        buffer[row].Add(myfunctions.getString(br.ReadString()));
                    else
                        buffer[row].Add(br.ReadString());
                }                
            }
            br.Close();
        }

        private void fill(List<int> indicesFields, List<int> fieldsSelected, int ini, int fin)
        {
            buffer.Clear();
            if (tempIndices == null || tempIndices.Count == 0)
                return;
            //creando el numero de filas como indices
            for (int i = ini; i < tempIndices.Count && i < fin; i++)
                buffer.Add(new List<string>());

            BinaryReader br;
            br = new BinaryReader(File.Open("BD/" + name + "/" + name + ".table", FileMode.Open));

            for (int i = ini; i < buffer.Count && i < fin; i++) // por cada fila
            {
                int lineOffset = tempIndices[i] * rowSize;
                int row = i - ini;
                for (int j = 0; j < indicesFields.Count; j++) // por cada campo seleccionado
                {
                    br.BaseStream.Seek(lineOffset + indicesFields[j], SeekOrigin.Begin);
                    if (myTypes[fieldsSelected[j]] == "integer")
                        buffer[row].Add(br.ReadInt32().ToString());
                    else if (myTypes[fieldsSelected[j]] == "boolean")
                        buffer[row].Add(br.ReadBoolean().ToString());
                    else if (myTypes[fieldsSelected[j]] == "varchar")
                        buffer[row].Add(myfunctions.getString(br.ReadString()));
                    else
                        buffer[row].Add(br.ReadString());
                }                
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

        public bool createIndex(string fieldName, bool isprimary)
        {
            List<string> listoffields = new List<string>();
            listoffields.Add(fieldName);
            fillBuffer(null,listoffields);
                        
            BTree temp = new BTree(fieldName, isprimary);

            if (buffer.Count == 0)
                return true;

            for (int row = 0; row < buffer.Count; row++)
            {
                if (!temp.insert(buffer[row][0], tempIndices[row]))
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

        public int findBtreeIndex(string field)
        {
            for (int i = 0; i < btrees.Count; i++)
            {
                if (String.Compare(btrees[i].name, field) == 0)
                    return i;
            }
            return -1;
        }

        public void getTableScandingIndices(List<string> where)
        {            
            BinaryReader br;
            int offset = findFieldOffset(where[0]);
            int fieldPos = isField(where[0]);
            br = new BinaryReader(File.Open("BD/" + name + "/" + name + ".table", FileMode.Open));
            string data = "";
            for (int i = 0; i < indices.Count; i++)
            {
                br.BaseStream.Seek((indices[i] * rowSize) + offset, SeekOrigin.Begin);
                if (myTypes[fieldPos] == "integer")
                    data = br.ReadInt32().ToString();
                else if (myTypes[fieldPos] == "boolean")
                    data = br.ReadBoolean().ToString();
                else if (myTypes[fieldPos] == "varchar")
                    data = myfunctions.getString(br.ReadString());
                else
                    buffer[i].Add(br.ReadString());
                if (String.Compare(data, where[2]) == 0)
                    tempIndices.Add(indices[i]);
            }
            br.Close();
        }

        private int findFieldOffset(string field_)
        {
            int pos = 0;
            for (int i = 0; i < myFields.Count; i++)
            {
                //Console.WriteLine(myTypes[i]);
                if (myFields[i] == field_)
                    return pos;
                if (myTypes[i] == "varchar")
                    pos += myfunctions.stringSize;
                else if (myTypes[i] == "integer")
                    pos += myfunctions.intSize;
                else if (myTypes[i] == "boolean")
                    pos += myfunctions.boolSize;
                else if (myTypes[i] == "date")
                    pos += myfunctions.dateSize;
            }
            return pos;
        }
    }
}
