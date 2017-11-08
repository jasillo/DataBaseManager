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

        public DBDescriptor()
        {
            myTables = new List<TableDescriptor>();
            errors = "";
        }

        public void save()
        {
            BinaryWriter bw;
            try
            {
                bw = new BinaryWriter(new FileStream("database.db", FileMode.Create));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }
            bw.Write("hola");
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
            return true;
        }

        public bool dropTable(string tableName)
        {
            for (int i = 0; i < myTables.Count; i++)
            {
                if (myTables[i].name == tableName)
                {
                    myTables.RemoveAt(i);
                    return true;
                }
            }
            errors += String.Format("No existe la tabla");
            return false;
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

    }
}
