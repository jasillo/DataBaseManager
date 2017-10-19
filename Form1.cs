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
        private Analyzer myLexical;
        public Form1()  
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            myLexical = new Analyzer();
        }

        private void tbnExecute_Click(object sender, EventArgs e)
        {
            string sql = tbSql.Text + ' ';
            sql = sql.Replace("\n", "");            
            myLexical.analizeSql(sql);
            if (String.IsNullOrEmpty (myLexical.errors))
                tbResult.Text = myLexical.errors;
            else
                tbResult.Text = myLexical.showListofNodes();            
        }
    }
}
