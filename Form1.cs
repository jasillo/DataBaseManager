using System;
using System.IO;
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
            string[] apellidos = { "Abad", "Abalos", "Abarca", "Abendano", "Abila", "Abina", "Abitua", "Aboites", "Abonce", "Abrego", "Abrica", "Abrigo",
                "Abundis","Aburto","Acebedo", "Acebes", "Acencio", "Adan", "Adrian", "Agirre", "Agredano", "Aguado", "Aguallo","Aguayo", "Agueda",
                "Aguero", "Baes", "Bahena", "Baina", "Baisa", "Baker", "Balades", "Balbaneda", "Balberde", "Balbuena", "Balderas", "Baldes","Baldespino"
                ,"Baldibia", "Baldivia", "Baldivieso","Baldonado","Baldovinos","Balencia","Balencuela","Balensuela","Balentin", "Balenzuela","Carlos",
                "Carmel", "Carmona", "Carnero", "Caro", "Carpintero", "Carpio", "Carrales","Carranco","Carrasco","Carreno","Carrera","Carreto","Carrillo"
                ,"Carrion","Carrisal","Carrisales","Carriyo","Carrizal","Carro", "Carvajal","Casa","Casanoba", "Casanova","Casares","Casasola","Dado",
                "Daniel","Dasa","Davila","Daza","Delara","Delgadillo","Delgado","Delos","Delossantos","Deras","Dimas","Dionicio","Dios","Dolores",
                "Domingues","Dominguez","Donate","Dongu","Dorado","Dorantes","Duarte","Duenas","Duque","Duran","Duron" };
            string[] nombres = { "Alberto", "Andres", "Antonio", "Benjamin", "Bruno", "Bryan", "Cristian", "David","Dylan","Eduardo","Enrique","Oscar",
                "Fabian","Felipe","Fernando","Francisco","Gael","Guillermo","Gustavo","Hector","Ian","Iker","Isaac","Ivan","Jesus","Joel","Jonathan",
                "Jorge","Jose","Kevin","Manuel","Martin","Mateo","Miguel","Nacho","Nicolas","Omar","Pedro","Rafael","Rafa","Ricardo","Roberto","Rodrigo",
                "Samuel","Sebastian","Thiago","Uriel","Victor","Alvaro"};
            Random rnd = new Random();
            string cadena = "";
            //myAnalyzer.db.myTables[1].tempbw =  new BinaryWriter(File.Open("BD/prueba/prueba.table", FileMode.Append));
            BinaryWriter bw = new BinaryWriter(File.Open("BD/estudiante/estudiante.table", FileMode.Append));
            for (int i = 0; i < 5000000; i++)
            {
            bw.Write(i);
            bw.Write(myfunctions.fixedString(nombres[rnd.Next(0,45)]));
            bw.Write(myfunctions.fixedString(apellidos[rnd.Next(0, 100)]));
            bw.Write(rnd.Next(15, 55));
            //cadena = String.Format("insert prueba {0} \"{1}\" \"{2}\" {3}; {4}", i, nombre[rnd.Next(0,20)],apellido[rnd.Next(0,28)],rnd.Next(15,55), Environment.NewLine);
            //cadena = String.Format("insert prueba {0} ; {1}", i, Environment.NewLine);
            //myAnalyzer.analizeSql(cadena);
            //myAnalyzer.analyzeNodes();
            }
            //myAnalyzer.db.myTables[1].tempbw.Close();
            bw.Close();

            //bw = new BinaryWriter(File.Open("BD/estudiante_x/estudiante_x.list", FileMode.Append));
            //bw.Write(50000000);
            myAnalyzer.db.myTables[0].indices.Clear();
            myAnalyzer.db.myTables[0].hollows.Clear();
            for (int i = 0; i < 5000000; i++)
            //{
                myAnalyzer.db.myTables[0].indices.Add(i);
            //}
            //bw.Write(0);
            //bw.Close();
            myAnalyzer.db.myTables[0].end = 5000000;
            */
            tbResult.Text = myAnalyzer.db.show();
            //myAnalyzer.db.myTables[0].btrees[0].show();
            //Console.WriteLine(String.Compare("1","1000000"));
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < myAnalyzer.db.myTables.Count; i++)
            {
                for (int j = 0; j < myAnalyzer.db.myTables[i].btrees.Count; j++)
                {
                    if (!myAnalyzer.db.myTables[i].btrees[j].memory)
                    {
                        myAnalyzer.db.myTables[i].btrees[j].memory = true;
                        myAnalyzer.db.myTables[i].btrees[j].loadMemory();
                    }                        
                }
            }
            tbResult.Text = "ARBOL EN MEMORIA";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < myAnalyzer.db.myTables.Count; i++)
            {
                for (int j = 0; j < myAnalyzer.db.myTables[i].btrees.Count; j++)
                {
                    if (myAnalyzer.db.myTables[i].btrees[j].memory)
                    {
                        myAnalyzer.db.myTables[i].btrees[j].memory = false;
                        myAnalyzer.db.myTables[i].btrees[j].nodes.Clear();
                    }                    
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            tbResult.Text = "ARBOL EN DISCO";
        }
    }
}
