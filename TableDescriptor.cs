using System;
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

        public TableDescriptor(string tableName)
        {
            name = tableName;
            fields = new List<string>();
            types = new List<string>();
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
            return true;
        }

        public string show()
        {
            string r = String.Format("Tabla {0} {1}",name, Environment.NewLine);
            for (int i = 0; i < fields.Count; i++)
            {
                r += String.Format("\t{0} {1}{2}",fields[i], types[i], Environment.NewLine);
            }
            return r;
        }

        
    }
}
