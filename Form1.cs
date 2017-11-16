﻿using System;
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
            Console.WriteLine(myAnalyzer.errors);
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
        {
            tbResult.Text = myAnalyzer.db.show();
            /*string[] apellido = { "Flores", "Rodríguez", "Sánchez", "García", "Rojas", "Díaz", "Torres",
                "López", "Gonzales", "Pérez", "Chávez", "Vásquez", "Ramos", "Ramírez", "Mendoza", "Espinoza",
                "Castillo", "Huamán", "Vargas", "Mamani", "Fernández", "Gutiérrez", "Ruiz", "Castro", "Romero",
                "Salazar", "Cruz","Gómez", "Rivera" };
            string[] nombre = { "jorge", "mauricio", "roger", "manuel", "jose", "alejandra", "maria", "rosario",
                "francisco", "rosa", "damian", "daniela", "edgar", "ronald", "diego", "omar", "alberto", "julio",
                "daniel", "sandro", "marcela"};
            Random rnd = new Random();
            string nombretabla = "persona";
            string cadena = String.Format("createtable {0} id integer nombre varchar apellido varchar edad integer ; {1}",
                nombretabla, Environment.NewLine);
            for (int i = 0; i < 10000; i++)
            {
                cadena += String.Format("insert {0} {1} {2} {3} {4};{5}", nombretabla, i, nombre[rnd.Next(0,20)],
                    apellido[rnd.Next(0,28)], rnd.Next(15, 55),Environment.NewLine);
            }
            tbResult.Text = cadena;*/
        }
        
    }
}
