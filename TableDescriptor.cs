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
        public List<string> fields;
        public List<string> types;
        public string name;
        public List <int> hollows;
        public int rowSize;
        public List<List<string>> result;

        public TableDescriptor(string tableName)
        {
            name = tableName;
            fields = new List<string>();
            types = new List<string>();
            hollows = new List<int>();
            result = new List<List<string>>();
            rowSize = 0;
        }

        public bool addField(string fieldName, string typeName)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] == fieldName)
                    return false;
            }
            fields.Add(fieldName);
            types.Add(typeName);
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
            for (int i = 0; i < fields.Count; i++)
            {
                r += String.Format("\t{0} {1}{2}",fields[i], types[i], Environment.NewLine);
            }
            r += String.Format("\trow size : {0} {1}", rowSize, Environment.NewLine);
            return r;
        }

        public void insertRow(List<string> types, List<string> values)
        {
            BinaryWriter bw;
            BinaryReader br;
            if (hollows.Count == 0)
            {
                bw = new BinaryWriter(File.Open(name + ".table", FileMode.Append));
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i] == "integer")
                        bw.Write(Int32.Parse(values[i]));
                    else if (types[i] == "boolean")
                        bw.Write(Boolean.Parse(values[i]));
                    else if (types[i] == "varchar")
                        bw.Write(myfunctions.fixedString(values[i]));
                    else
                        bw.Write(values[i]);

                }
                bw.Close();
            }
            else
            {
                //bw = new BinaryWriter(new FileStream(name + ".table", FileMode.Append));
            }
            //bw.Close();
        }

        public string select(List<int> indices)
        {
            string r = "";
            BinaryReader br;
            br = new BinaryReader(File.Open(name + ".table", FileMode.Open));
            long start = 0;
            long end = br.BaseStream.Length;
            while (start < end)
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    br.BaseStream.Seek(start + indices[i], SeekOrigin.Begin);
                    if (fields[indices[i]] == "integer")
                        r += br.ReadInt32().ToString();
                    else if (fields[indices[i]] == "boolean")
                        r += br.ReadBoolean().ToString();
                    else if (fields[indices[i]] == "varchar")
                        r += myfunctions.getString(br.ReadString());
                    else
                        r += br.ReadString();
                    r += '\t';
                }                
                start += rowSize;
            }
            br.Close();
            return r;
        }

        public int isField(string field)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] == field)
                    return i;
            }
            return -1;
        }
        
    }
}
