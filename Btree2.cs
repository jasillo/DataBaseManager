using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    class Btree2
    {
        public long lastPosition;
        public string field;
        public bool primary;
        public string table;

        Btree2(string table_, string field_, bool primary_)
        {
            table = table_;
            field = field_;
            primary = primary_;

            if (!File.Exists("BD/" + table + "/" + field + ".btree"))
            {
                BinaryWriter bw = new BinaryWriter(new FileStream("BD/" + table + "/" + field + ".btree", FileMode.Create));
                bw.Write((long)8);
                bw.Close();
            }
        }

        void insert(string key, int index)
        {

        }

    }
}
