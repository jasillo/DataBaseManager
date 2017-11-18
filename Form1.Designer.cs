namespace DataBaseManager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbSql = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.tbnExecute = new System.Windows.Forms.Button();
            this.btnVerTablas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbSql
            // 
            this.tbSql.AcceptsTab = true;
            this.tbSql.Location = new System.Drawing.Point(36, 58);
            this.tbSql.Multiline = true;
            this.tbSql.Name = "tbSql";
            this.tbSql.Size = new System.Drawing.Size(376, 355);
            this.tbSql.TabIndex = 0;
            // 
            // tbResult
            // 
            this.tbResult.AcceptsTab = true;
            this.tbResult.Location = new System.Drawing.Point(445, 58);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(542, 355);
            this.tbResult.TabIndex = 1;
            // 
            // tbnExecute
            // 
            this.tbnExecute.Location = new System.Drawing.Point(36, 29);
            this.tbnExecute.Name = "tbnExecute";
            this.tbnExecute.Size = new System.Drawing.Size(75, 23);
            this.tbnExecute.TabIndex = 2;
            this.tbnExecute.Text = "Ejecutar";
            this.tbnExecute.UseVisualStyleBackColor = true;
            this.tbnExecute.Click += new System.EventHandler(this.tbnExecute_Click);
            // 
            // btnVerTablas
            // 
            this.btnVerTablas.Location = new System.Drawing.Point(445, 29);
            this.btnVerTablas.Name = "btnVerTablas";
            this.btnVerTablas.Size = new System.Drawing.Size(75, 23);
            this.btnVerTablas.TabIndex = 3;
            this.btnVerTablas.Text = "Ver tablas";
            this.btnVerTablas.UseVisualStyleBackColor = true;
            this.btnVerTablas.Click += new System.EventHandler(this.btnVerTablas_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 431);
            this.Controls.Add(this.btnVerTablas);
            this.Controls.Add(this.tbnExecute);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.tbSql);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSql;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Button tbnExecute;
        private System.Windows.Forms.Button btnVerTablas;
    }
}

