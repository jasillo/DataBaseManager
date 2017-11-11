using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{    
    partial class Analyzer
    {
        public string errors;
        public string results;
        public List<Node> myNodes;
        public DBDescriptor db;
        
        public Analyzer()
        {
            myNodes = new List<Node>();
            errors = "";
            db = new DBDescriptor();
        }        
            
        public void analizeSql(string text)
        {
            myNodes.Clear();
            errors = "";

            int line = 1;
            int state = 0;
            Token tempToken;
            char character;
            string word = "";
            errors = "";

            for (int i = 0; i < text.Length; i++)
            {
                character = text[i];
                tempToken = isBreaker(character);
                switch (state)
                {
                    case 0: //cadena vacia
                        word = "";
                        if (tempToken > Token.vacio)
                        {                            
                            saveNode(character.ToString(), tempToken, line);
                            break;
                        }
                        if (Char.IsLetter(character))
                        {
                            state = 1;
                            word += character;
                        }
                        else if (Char.IsDigit(character))
                        {
                            state = 2;
                            word += character;
                        }
                        else if (character == '\r')                        
                            line++;
                        else if (character == ' ')
                            break;
                        else if (character == '"')
                            state = 3;                                                
                        else
                        {
                            state = 99;
                            word += character;
                        }
                        break;
                    case 1: //identificadores
                        if (tempToken != Token.vacio)
                        {
                            saveNode(word, Token.identificador, line);
                            saveNode(character.ToString(), tempToken, line);                            
                            state = 0;
                            break;
                        }
                        if (Char.IsLetter(character))
                            word += character;
                        else if (character == ' ')
                        {
                            saveNode(word, Token.identificador, line);
                            state = 0;
                        }
                        else if (character == '\r')
                        {
                            saveNode(word, Token.identificador, line);
                            line++;
                            state = 0;
                        }
                        else if (character == '"')
                        {
                            saveNode(word, Token.identificador, line);
                            state = 3;
                        }                            
                        else
                            state = 99;
                        break;
                    case 2: //numbers
                        if (tempToken > Token.vacio)
                        {
                            saveNode(word, Token.integer, line);
                            saveNode(character.ToString(), tempToken, line);
                            state = 0;
                            break;
                        }
                        if (Char.IsDigit(character))
                            word += character;
                        else if (character == ' ')
                        {
                            saveNode(word, Token.integer, line);
                            state = 0;
                        }
                        else if (character == '\r')
                        {                            
                            saveNode(word, Token.integer, line);
                            line++;
                            state = 0;
                        }
                        else if (character == '"')
                        {
                            saveNode(word, Token.integer, line);
                            state = 3;
                        }
                        else
                            state = 99;
                        break;
                    case 3:
                        if (character == '"')
                        {
                            saveNode(word, Token.varchar, line);
                            state = 0;
                        }
                        else if (character == '\r')
                        {
                            state = 0;
                            saveNode(word, Token.vacio, line);
                        }
                        else
                            word += character;                        
                        break;
                    case 99:
                        if (tempToken > Token.vacio)
                        {
                            saveNode(word, Token.vacio, line);
                            saveNode(character.ToString(), tempToken, line);
                            state = 0;
                            break;
                        }
                        else if (character == ' ')
                        {
                            saveNode(word, Token.vacio, line);
                            state = 0;
                        }
                        else if (character == '\r')
                        {
                            saveNode(word, Token.vacio, line);
                            line++;
                            state = 0;
                        }
                        else if (character == '"')
                        {
                            state = 0;
                            saveNode(word, Token.vacio, line);
                        }
                        else                        
                            word += character;
                        break;
                    default:
                        break;
                }
            }
        }
                
        private Token isBreaker(char character)
        {
            if (character == ';')
                return Token.puntoComa;
            else if (character == '=')
                return Token.igual;
            else if (character == '*')
                return Token.pcAll;
            else if (character == '<')
                return Token.menor;
            else if (character == '>')
                return Token.mayor;
            return Token.vacio;
        }

        private void saveNode(string word, Token token, int line)
        {           
            if (String.IsNullOrEmpty(word.Trim()))
                return;
             if (token == Token.vacio)
                errors += String.Format("Error en linea {0}: {1} {2}",line,word,Environment.NewLine);
            Token t = Token.identificador;
            if (token != Token.identificador)
            {                
                myNodes.Add(new Node(token, line, word));
                return;
            }

            if (word.ToLower() == "createtable")
                t = Token.pcCreateTable;
            if (word.ToLower() == "createindex")
                t = Token.pcCreateIndex;
            if (word.ToLower() == "on")
                t = Token.pcOn;
            else if (word.ToLower() == "droptable")
                t = Token.pcDropTable;
            else if (word.ToLower() == "select")
                t = Token.pcSelect;
            else if (word.ToLower() == "from")
                t = Token.pcFrom;
            else if (word.ToLower() == "where")
                t = Token.pcWhere;
            else if (word.ToLower() == "insert")
                t = Token.pcInsert;
            else if (word.ToLower() == "update")
                t = Token.pcUpdate;
            else if (word.ToLower() == "set")
                t = Token.pcSet;
            else if (word.ToLower() == "delete")
                t = Token.pcDelete;
            else if (word.ToLower() == "varchar")
                t = Token.pcVarChar;
            else if (word.ToLower() == "integer")
                t = Token.pcInteger;
            else if (word.ToLower() == "boolean")
                t = Token.pcBoleano;
            else if (word.ToLower() == "date")
                t = Token.pcFecha;
            else if (word.ToLower() == "true")
                t = Token.boolean;
            else if (word.ToLower() == "false")
                t = Token.boolean;
            myNodes.Add(new Node(t, line, word));
        }

        public void showListofNodes()
        {
            string result = "";
            for (int i = 0; i < myNodes.Count; i++)
            {
                result = String.Format("dato: {0}\t token: {1}\t linea: {2}{3}",
                    myNodes[i].data, myNodes[i].token, myNodes[i].line, Environment.NewLine);
                Console.WriteLine(result);
            } 
        }
        
        
    } //fin de clase
}
