
namespace DataBaseManager
{
    enum Token
    {
        vacio,        
        pcVarChar, //2
        pcInteger, //3
        pcBoleano, //4
        pcFecha,    //5
        varchar,
        integer,
        boolean,
        date,
        pcCreateTable,
        pcDropTable,
        pcCreateIndex,
        pcDropIndex,
        pcSelect,
        pcFrom,
        pcWhere,
        pcInsert,
        pcUpdate,
        pcSet,
        pcDelete,
        pcAll,
        pcOn,
        pcPrimary,
        pcSecondary,
        identificador,
        puntoComa,
        igual,
        mayor,
        menor
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

        public Node(Node n)
        {
            token = n.token;
            line = n.line;
            data = n.data;
        }

        
    }

    public static class myfunctions
    {
        public static int stringSize = 12;
        public static int intSize = sizeof(int);
        public static int dateSize = 9;
        public static int boolSize = 1;

        public static string fixedString(string word)
        {
            int tam = stringSize - word.Length-1;
            string relleno = new string('*', tam);
            string r = relleno + word;
            return r;
        }

        public static string getString(string word)
        {
            string r = word.Replace('*', ' ');
            return r.Trim();
        }        
    }
}
