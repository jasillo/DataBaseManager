using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{    
    class Analyzer
    {
        public string errors;
        public List<Node> myNodes;
        
        public Analyzer()
        {
            myNodes = new List<Node>();
            errors = "";
        }
            
        public void analizeSql(string text)
        {
            myNodes.Clear();
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
                            saveNode(word, Token.entero, line);
                            saveNode(character.ToString(), tempToken, line);
                            state = 0;
                            break;
                        }
                        if (Char.IsDigit(character))
                            word += character;
                        else if (character == ' ')
                        {
                            saveNode(word, Token.entero, line);
                            state = 0;
                        }
                        else if (character == '\r')
                        {                            
                            saveNode(word, Token.entero, line);
                            line++;
                            state = 0;
                        }
                        else if (character == '"')
                        {
                            saveNode(word, Token.entero, line);
                            state = 3;
                        }
                        else
                            state = 99;
                        break;
                    case 3:
                        if (character == '"')
                        {
                            saveNode(word, Token.cadena, line);
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
            return Token.vacio;
        }

        private void saveNode(string word, Token token, int line)
        {
            if (token == Token.vacio)
                errors += String.Format("Error en linea {0}: {1} {2}",line,word,Environment.NewLine);
            if (String.IsNullOrEmpty(word.Trim()))
                return;

            Token t = Token.identificador;
            if (token != Token.identificador)
            {                
                myNodes.Add(new Node(token, line, word));
                return;
            }

            if (word.ToLower() == "createtable")
                t = Token.pcCreateTable;
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
            else if (word.ToLower() == "true")
                t = Token.boleano;
            else if (word.ToLower() == "false")
                t = Token.boleano;
            myNodes.Add(new Node(t, line, word));
        }

        public string showListofNodes()
        {
            string result = "";
            for (int i = 0; i < myNodes.Count; i++)
                result += String.Format("dato: {0}\t token: {1}\t linea: {2}{3}", 
                    myNodes[i].data, myNodes[i].token, myNodes[i].line, Environment.NewLine);            
            return result;
        }
        
        public void analyzeNodes()
        {
            List<Node> types = new List<Node>();
            List<Node> values = new List<Node>();
            List<Node> fields = new List<Node>();
            Node tableName;
            List<Node> where = new List<Node>(3);
            List<Node> set = new List<Node>(3);
            int state = 0;
            for (int i = 0; i < myNodes.Count; i++)
            {
                switch (state)
                {
                    case 0:
                        if (myNodes[1].token == Token.pcCreateTable)
                            state = 1;
                        else if (myNodes[1].token == Token.pcDropTable)
                            state = 11;
                        else if (myNodes[1].token == Token.pcDelete)
                            state = 21;
                        break;
                    case 1:
                        if (myNodes[1].token != Token.identificador)                        
                            errors += String.Format("Error en linea {0}, se esperaba nombre Tabla, e obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[1];
                        state = 2;
                        break;
                    case 2:
                        if (myNodes[1].token == Token.puntoComa)
                        {
                            if (!String.IsNullOrEmpty(errors))
                                return;
                            //ejecutar query
                            state = 0;
                        }
                        else if (myNodes[1].token == Token.identificador)
                        {
                            fields.Add(myNodes[1]);
                            state = 3;
                        }
                        else
                        {
                            errors += String.Format("Error en linea {0}, se esperaba nombre campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                            state = 3;
                        }
                        break;
                    case 3:
                        if (myNodes[1].token < Token.pcVarChar || myNodes[1].token >Token.pcBoleano)
                            errors += String.Format("Error en linea {0}, se esperaba tipo campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 2;
                        break;
                    case 11:
                        if (myNodes[1].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 12;
                        break;
                    case 12:
                        if (myNodes[1].token != Token.puntoComa)
                            errors += String.Format("Error en linea {0}, se esperaba punto y coma, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        state = 0;
                        break;
                    case 21:
                        if (myNodes[1].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 22;
                        break;
                    case 22:
                        if (myNodes[i].token != Token.pcWhere)
                            errors += String.Format("Error en linea {0}, se esperaba WHERE, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 23;
                        break;
                    case 23:
                        break;
                    default:
                        break;
                }
            }            
        }
    }
}
