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
            load();
        }

        ~DBDescriptor()
        {
            save();
        }

        private void load()
        {
            BinaryReader br;
            try
            {
                br = new BinaryReader( new FileStream("database.db", FileMode.Open) );
                int tam = br.ReadInt32();
                for (int i = 0; i < tam; i++)
                {
                    string tablename = br.ReadString();
                    TableDescriptor mytable = new TableDescriptor( myfunctions.getString( tablename ) );
                    int fieldscount = br.ReadInt32();
                    for (int j = 0; j < fieldscount; j++)
                    {
                        string field = br.ReadString();
                        string type = br.ReadString();
                        mytable.addField(myfunctions.getString (field),myfunctions.getString(type) );
                    }
                    myTables.Add(mytable);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return;
            }
            br.Close();

            for (int i = 0; i < myTables.Count; i++)
            {
                myTables[i].loadLists();
            }
        }

        public void save()
        {
            BinaryWriter bw;
            try
            {
                bw = new BinaryWriter(new FileStream("database.db", FileMode.Create));
                bw.Write(myTables.Count);
                for (int i = 0; i < myTables.Count; i++)
                {
                    bw.Write(myfunctions.fixedString(myTables[i].name));
                    bw.Write(myTables[i].myFields.Count);
                    for (int j = 0; j < myTables[i].myFields.Count; j++)
                    {
                        bw.Write( myfunctions.fixedString( myTables[i].myFields[j] ) );
                        bw.Write( myfunctions.fixedString( myTables[i].myTypes[j] ) );
                    }                    
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }
            //bw.Write("hola");
            bw.Close();
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
            BinaryWriter bw;
            bw = new BinaryWriter(new FileStream(tableName+".table", FileMode.Create));
            bw.Close();
            bw = new BinaryWriter(new FileStream(tableName + ".list", FileMode.Create));
            bw.Close();
            td.saveLists();
            return true;
        }

        public bool dropTable(string tableName)
        {
            for (int i = 0; i < myTables.Count; i++)
            {
                if (myTables[i].name == tableName)
                {
                    myTables.RemoveAt(i);
                    File.Delete(tableName + ".table");
                    File.Delete(tableName + ".list");
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

            myTables[index].insertRow(values);  
              
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
                    Console.WriteLine(j);
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
    }
}
