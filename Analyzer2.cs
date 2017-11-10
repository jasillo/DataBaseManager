using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{  
    partial class Analyzer
    {        
        public void analyzeNodes()
        {
            List<string> types = new List<string>();
            List<string> values = new List<string>();
            List<string> fields = new List<string>();
            string tableName = "";
            List<string> where = new List<string>(3);
            List<string> set = new List<string>(3);
            int tempState1 = 0;
            int tempState2 = 0;
            string results = "";

            int state = 0;

            for (int i = 0; i < myNodes.Count; i++)
            {
                //Console.WriteLine(state);
                switch (state)
                {
                    case 0:
                        
                        types.Clear();
                        values.Clear();
                        fields.Clear();
                        where.Clear();
                        set.Clear();
                        tableName = "";
                        tempState1 = 0;
                        tempState2 = 0;

                        if (myNodes[i].token == Token.pcCreateTable)
                            state = 1;
                        else if (myNodes[i].token == Token.pcDropTable)
                            state = 11;
                        else if (myNodes[i].token == Token.pcDelete)
                            state = 21;
                        else if (myNodes[i].token == Token.pcUpdate)
                            state = 31;
                        else if (myNodes[i].token == Token.pcInsert)
                            state = 41;
                        else if (myNodes[i].token == Token.pcSelect)
                            state = 51;
                        break;

                    /////////////////////////////////////////////////////////////////
                    case 1: //came from createtable
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre Tabla, e obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 2;
                        break;
                    case 2:
                        if (myNodes[i].token == Token.puntoComa)
                            goto case 4;
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        fields.Add(myNodes[i].data);
                        state = 3;                        
                        break;                    
                    case 3:
                        if (myNodes[i].token < Token.pcVarChar || myNodes[i].token > Token.pcBoleano)
                            errors += String.Format("Error en linea {0}, se esperaba tipo campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        types.Add(myNodes[i].data);
                        state = 2;
                        break;
                    case 4:
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        if (!db.createTable(tableName,fields,types))
                        {
                            errors += String.Format("Error al crear tabla {0}, {1}{2}", tableName,db.errors, Environment.NewLine);
                            return;
                        }
                        db.save();
                        Console.WriteLine("create correcto");
                        state = 0;
                        break;

                    /////////////////////////////////////////////////////////////
                    case 11: //came from droptable
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 12;
                        break;
                    case 12:
                        Console.WriteLine("entre");
                        if (myNodes[i].token != Token.puntoComa)
                            errors += String.Format("Error en linea {0}, se esperaba punto y coma, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        if (!db.dropTable(tableName))
                        {
                            errors += String.Format("Error al borrar tabla {0}, {1}{2}", tableName, db.errors, Environment.NewLine);
                            return;
                        }
                        db.save();
                        Console.WriteLine("drop correcto");
                        state = 0;
                        break;

                    /////////////////////////////////////////////////////
                    case 21: //came from delete
                        if (myNodes[i].token != Token.pcFrom)
                            errors += String.Format("Error en linea {0}, se esperaba palabra clave FROM, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 22;
                        break;
                    case 22:
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 111; //where
                        tempState1 = 23;
                        break;                    
                    case 23:
                        if (myNodes[i].token != Token.puntoComa)
                            errors += String.Format("Error en linea {0}, se esperaba un valor, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        Console.WriteLine("delete correcto");
                        state = 0;
                        break;

                    ///////////////////////////////////////////////////////////
                    case 31: //came from update
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 101; //set
                        tempState1 = 111; //where
                        tempState2 = 32;
                        break;
                    case 32:
                        if (myNodes[i].token != Token.puntoComa)
                            errors += String.Format("Error en linea {0}, se esperaba punto coma, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        Console.WriteLine("update correcto");
                        state = 0;
                        break;

                    ////////////////////////////////////////////////////////////////
                    case 41://came from insert
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 42;
                        break;
                    case 42:
                        if (myNodes[i].token == Token.puntoComa)
                            goto case 43;
                        if (myNodes[i].token < Token.varchar || myNodes[i].token > Token.date)
                            errors += String.Format("Error en linea {0}, se esperaba valor, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        values.Add(myNodes[i].data);
                        types.Add(myNodes[i].token.ToString());
                        break;
                    case 43:
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        if ( !db.insertRow(tableName, types, values))
                        {
                            errors += String.Format("Error al insertar en tabla {0}, {1}{2}", tableName, db.errors, Environment.NewLine);
                            return;
                        }

                        Console.WriteLine("insert correcto");
                        state = 0;
                        break;

                    ///////////////////////////////////////////////////////////
                    case 51: //came from select
                        if (myNodes[i].token == Token.pcFrom)
                        {
                            state = 52;
                            break;
                        }
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba valor, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        fields.Add(myNodes[i].data);
                        break;
                    case 52:
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre tabla, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        tableName = myNodes[i].data;
                        state = 111;
                        tempState2 = 53;
                        break;
                    case 53:                        
                        if (myNodes[i].token != Token.puntoComa)
                            errors += String.Format("Error en linea {0}, se esperaba punto coma, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        if (!String.IsNullOrEmpty(errors))
                            return;
                        //ejecutar query
                        Console.WriteLine("select correcto");
                        state = 0;
                        break;

                    //////////////////////////////////////////////////////////////
                    case 101: // caso set
                        if (myNodes[i].token != Token.pcSet)
                            errors += String.Format("Error en linea {0}, se esperaba palabra clave SET, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 102;
                        break;
                    case 102: // caso set
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        set.Add(myNodes[i].data);
                        state = 103;
                        break;
                    case 103: // caso set
                        if (myNodes[i].token != Token.igual)
                            errors += String.Format("Error en linea {0}, se esperaba igual, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        set.Add(myNodes[i].data);
                        state = 104;
                        break;
                    case 104: // caso set
                        if (myNodes[i].token < Token.varchar || myNodes[i].token > Token.date)
                            errors += String.Format("Error en linea {0}, se esperaba valor, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        set.Add(myNodes[i].data);
                        state = tempState1;
                        break;

                    ///////////////////////////////////////////////////////////////
                    case 111: // caso where
                        if (myNodes[i].token == Token.puntoComa)
                            goto case tempState2;
                        if (myNodes[i].token != Token.pcWhere)
                            errors += String.Format("Error en linea {0}, se esperaba palabra clave WHERE, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        state = 112;
                        break;
                    case 112: // caso where
                        Console.WriteLine("aqui");
                        if (myNodes[i].token != Token.identificador)
                            errors += String.Format("Error en linea {0}, se esperaba nombre campo, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        where.Add(myNodes[i].data);
                        state = 113;
                        break;
                    case 113: // caso where
                        if (myNodes[i].token < Token.igual || myNodes[i].token > Token.menor)
                            errors += String.Format("Error en linea {0}, se esperaba simbolo comparacion, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        where.Add(myNodes[i].data);
                        state = 114;
                        break;
                    case 114: // caso where
                        if (myNodes[i].token < Token.varchar || myNodes[i].token > Token.date)
                            errors += String.Format("Error en linea {0}, se esperaba valor, se obtuvo {1}{2}",
                                myNodes[i].line, myNodes[i].data, Environment.NewLine);
                        where.Add(myNodes[i].data);
                        state = tempState2;
                        break;
                    default:
                        break;
                }//fin de swich
            }//fin de for
        }//fin analyzeNodes

    }//fin de clase
}
