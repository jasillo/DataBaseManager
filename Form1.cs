using System;
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
        }
    }
}
