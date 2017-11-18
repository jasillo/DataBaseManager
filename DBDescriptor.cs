using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    class DBDescriptor
    {
        List<TableDescriptor> myTables;
        public string errors;
        public string results;

        public DBDescriptor()
        {
            myTables = new List<TableDescriptor>();
            errors = "";
            results = "";
            load();
        }

        ~DBDescriptor()
        {
            save();
        }

        private void load()
        {
            BinaryReader br;
            if (File.Exists("BD/database.db"))
            {
                br = new BinaryReader(new FileStream("BD/database.db", FileMode.Open));
                //leer los descriptores de todas las tablas en la BD
                int numberOfTables = br.ReadInt32();
                for (int tableIndex = 0; tableIndex < numberOfTables; tableIndex++)
                {
                    string tablename = br.ReadString();
                    myTables.Add(new TableDescriptor(tablename));
                    int fieldscount = br.ReadInt32();
                    for (int j = 0; j < fieldscount; j++)
                    {
                        myTables[tableIndex].myFields.Add(br.ReadString());
                        myTables[tableIndex].myTypes.Add(br.ReadString());
                    }
                    myTables[tableIndex].rowSize = br.ReadInt32();
                    myTables[tableIndex].end = br.ReadInt32();
                    int indexCount = br.ReadInt32();
                    for (int i = 0; i < indexCount; i++)                    
                        myTables[tableIndex].btrees.Add(new BTree(br.ReadString()));                    
                }
                br.Close();

                for (int tableIndex = 0; tableIndex < myTables.Count; tableIndex++)                
                    myTables[tableIndex].load();
                
            }                              
        }

        public void save()
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("BD/database.db", FileMode.Create));
            bw.Write(myTables.Count);
            for (int tableIndex = 0; tableIndex < myTables.Count; tableIndex++)
            {
                bw.Write(myTables[tableIndex].name);
                bw.Write(myTables[tableIndex].myFields.Count);
                for (int  column = 0; column < myTables[tableIndex].myFields.Count; column++)
                {
                    bw.Write(myTables[tableIndex].myFields[column]);
                    bw.Write(myTables[tableIndex].myTypes[column]);
                }
                bw.Write(myTables[tableIndex].rowSize);
                bw.Write(myTables[tableIndex].end);
                bw.Write(myTables[tableIndex].btrees.Count);
                for (int btreeIndex =0; btreeIndex < myTables[tableIndex].btrees.Count;btreeIndex++)                
                    bw.Write(myTables[tableIndex].btrees[btreeIndex].name);                
            }
            bw.Close();

            for (int tableindex = 0; tableindex < myTables.Count; tableindex++)   
                myTables[tableindex].save();            
        }


        public bool createTable(string tableName, List<string> values, List<string> types)
        {
            errors = "";
            if (values.Count != types.Count)
            {
                errors += String.Format("numero de valores y tipos no coincide");
                return false;
            }
            for (int i = 0; i < myTables.Count; i++)
            {
                if (myTables[i].name == tableName)
                {
                    errors += String.Format("ya existe la tabla");
                    return false;
                }
            }
            TableDescriptor td = new TableDescriptor(tableName);
            for (int i = 0; i < values.Count; i++)
            {
                if (!td.addField(values[i], types[i]))
                {
                    errors += String.Format("campo repetido");
                    return false;
                }
            }
            myTables.Add(td);
            Directory.CreateDirectory("BD/"+ tableName);
            BinaryWriter br = new BinaryWriter(new FileStream("BD/" + tableName + "/" + tableName + ".table", FileMode.Create));
            br.Close();
            return true;
        }

        public bool dropTable(string tableName)
        {
            for (int i = 0; i < myTables.Count; i++)
            {
                if (myTables[i].name == tableName)
                {
                    myTables.RemoveAt(i);
                    if (Directory.Exists("BD/" + tableName))
                        Directory.Delete("BD/"+ tableName,true);
                    return true;
                }
            }
            errors += String.Format("No existe la tabla");
            return false;
        }

        public bool insertRow(string tablename,List<string> types, List<string> values)
        {
            int index = findTable(tablename);
            if (index == -1)
            {
                errors += String.Format("tabla no existe");
                return false;
            }                
            if (types.Count != myTables[index].myFields.Count)
            {
                errors += String.Format("numero de campos no coincide");
                return false;
            }
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] != myTables[index].myTypes[i])
                {
                    errors += String.Format("el tipo de los valores no coincide");
                    return false;
                }
            }

            if (!myTables[index].insertRow(values))
            {
                errors += String.Format("violacion de indice primario");
                return false;
            }
            return true;
        }

        public bool select(string tablename, List<string> fields, List<string> where)
        {
            int index = findTable(tablename);
            if (index == -1)                           
                return false;
            //consigue los indices de los columnas
            if (fields.Count == 0)
                return true;
            List<int> listoffield = new List<int>();
            if (fields[0] == "*")
            {
                for (int i = 0; i < myTables[index].myFields.Count; i++)
                    listoffield.Add(i);
            }
            else
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    int j = myTables[index].isField(fields[i]);
                    if (j == -1)
                    {
                        errors += String.Format("no existe el campo {0}", fields[i]);
                        return false;
                    }
                    listoffield.Add(j);
                }
            }           
            //busca dentro de los datos fisicos y retorna su resultado
            myTables[index].fillBuffer(where);
            results = myTables[index].select(listoffield);
            return true;
        }

        public bool delete(string tablename, List<string> where)
        {
            int index = findTable(tablename);
            if (index == -1)                         
                return false;
            //comprobar si existe la columna
            int columnIndex = myTables[index].isField(where[0]);
            if (columnIndex == -1)
            {
                errors += String.Format("no existe el campo {0}", where[0]);
                return false;
            }
            //comprobar si es el mismo tipo de dato que el de la tabla
            Console.WriteLine("tipos : {0}{1}", myTables[index].myTypes[columnIndex], where[3]);
            if (myTables[index].myTypes[columnIndex] != where[3])
            {
                errors += String.Format("error de tipo en el campo {0}", where[0]);
                return false;
            }
            //llenar el buffer
            myTables[index].fillBuffer(where);
            myTables[index].deleteBuffer();

            return true;
        }

        public string show()
        {
            string r = String.Format("Tablas : {0}", Environment.NewLine);
            for (int i = 0; i < myTables.Count; i++)
            {
                r += myTables[i].show();
            }
            return r;
        }

        int findTable(string tablename)
        {
            for (int i = 0; i < myTables.Count; i++)
            {
                if (myTables[i].name == tablename)
                    return i;
            }
            errors += String.Format("No existe la tabla");
            return -1;
        }

        public bool createIndex(string tableName, string fieldName, bool isprimary)
        {
            int tableIndex = findTable(tableName);
            if (tableIndex == -1)
            {
                errors += String.Format("no existe la tabla");
                return false;
            }
            //ver si existe indice para ese campo
            int index = myTables[tableIndex].findFieldIndex(fieldName);
            if (index > -1)
            {
                errors += String.Format("ya existe el indice");
                return false;
            }

            if (!myTables[tableIndex].createIndex(fieldName, isprimary))
            {
                errors += String.Format("es primario y necesita tener a tabla vacia");
                return false;
            }

            return true;
        }

        public bool dropIndex(string tableName, string fieldName)
        {
            int tableIndex = findTable(tableName);
            if (tableIndex == -1)
            {
                errors += String.Format("NO existe la tabla");
                return false;
            }
            if (myTables[tableIndex].dropIndex(fieldName))
                return true;
            
            errors += String.Format("NO existe el indice");
            return false;
        }
    }
}
