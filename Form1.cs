using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseManager
{
    public partial class Form1 : Form
    {
        private Analyzer myAnalyzer;
        BTree prueba;

        public Form1()  
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            myAnalyzer = new Analyzer();
            prueba = new BTree("hola");
        }
        
        private void tbnExecute_Click(object sender, EventArgs e)
        {
            tbResult.Text = "";
            string sql = tbSql.Text + ' ';
            sql = sql.Replace("\n", "");
            myAnalyzer.analizeSql(sql);
            //myLexical.showListofNodes();
            //Console.WriteLine(myAnalyzer.errors);
            if (!String.IsNullOrEmpty(myAnalyzer.errors))
            {
                tbResult.Text = "lexical errors : \r" + myAnalyzer.errors;
                return;
            }
            myAnalyzer.analyzeNodes();
            if (!String.IsNullOrEmpty(myAnalyzer.errors))
            {
                tbResult.Text = "sintactic errors : \r" + myAnalyzer.errors;
                return;
            }
            tbResult.Text = myAnalyzer.results;
                       
        }

        private void btnVerTablas_Click(object sender, EventArgs e)
        {/*
            //BinaryWriter bw = new BinaryWriter(File.Open("BD/estudiante/estudiante.table", FileMode.Append));
            string[] apellido = { "Flores", "Rodriguez", "Sanchez", "Garcia", "Rojas", "Diaz", "Torres",
                "Lopez", "Gonzales", "Perez", "Chavez", "Vasquez", "Ramos", "Ramirez", "Mendoza", "Espinoza",
                "Castillo", "Huaman", "Vargas", "Mamani", "Fernandez", "Gutierrez", "Ruiz", "Castro", "Romero",
                "Salazar", "Cruz","Gomez", "Rivera" };
            string[] nombre = { "jorge", "mauricio", "roger", "manuel", "jose", "alejandra", "maria", "rosario",
                "francisco", "rosa", "damian", "daniela", "edgar", "ronald", "diego", "omar", "alberto", "julio",
                "daniel", "sandro", "marcela"};
            Random rnd = new Random();
            string cadena = "";
            myAnalyzer.db.myTables[1].tempbw =  new BinaryWriter(File.Open("BD/prueba/prueba.table", FileMode.Append));
            //BinaryWriter bw = new BinaryWriter(File.Open("BD/estudiantex/estudiantex.table", FileMode.Append));
            for (int i = 0; i < 100; i++)
            {
            //bw.Write(i);
            //bw.Write(myfunctions.fixedString(nombre[rnd.Next(0,20)]));
            //bw.Write(myfunctions.fixedString(apellido[rnd.Next(0, 28)]));
            //bw.Write(rnd.Next(15, 55));
            //cadena = String.Format("insert prueba {0} \"{1}\" \"{2}\" {3}; {4}", i, nombre[rnd.Next(0,20)],apellido[rnd.Next(0,28)],rnd.Next(15,55), Environment.NewLine);
            cadena = String.Format("insert prueba {0} ; {1}", i, Environment.NewLine);
            myAnalyzer.analizeSql(cadena);
            myAnalyzer.analyzeNodes();
            }
            myAnalyzer.db.myTables[1].tempbw.Close();
            //bw.Close();

            //bw = new BinaryWriter(File.Open("BD/estudiante_x/estudiante_x.list", FileMode.Append));
            //bw.Write(50000000);
            //myAnalyzer.db.myTables[1].indices.Clear();
            //myAnalyzer.db.myTables[1].hollows.Clear();
            //for (int i = 0; i < 50000000; i++)
            //{
                //myAnalyzer.db.myTables[1].indices.Add(i * 40);
            //}
            //bw.Write(0);
            //bw.Close();
            //myAnalyzer.db.myTables[1].end = 2000000000;
            */
            tbResult.Text = myAnalyzer.db.show();
            //myAnalyzer.db.myTables[1].btrees[0].show();
            //Console.WriteLine(String.Compare("1","1000000"));
            
            
        }
        
    }
}
