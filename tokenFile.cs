using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    enum Token
    {
        vacio,        
        pcVarChar, //2
        pcInteger, //3
        pcBoleano, //4
        pcFecha,    //5
        cadena,
        entero,
        boleano,
        pcCreateTable,
        pcDropTable,
        pcSelect,
        pcFrom,
        pcWhere,
        pcInsert,
        pcUpdate,
        pcSet,
        pcDelete,
        pcAll,
        identificador,
        puntoComa,
        igual
    };

    class Node
    {
        public Token token;
        public int line;
        public string data;

        public Node(Token t, int l, string d)
        {
            token = t;
            line = l;
            data = d;
        }
    }
}
