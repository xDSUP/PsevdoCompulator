namespace lab1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.AnalizTextBox = new System.Windows.Forms.TextBox();
            this.ResultTextBox = new System.Windows.Forms.TextBox();
            this.AnalBut = new System.Windows.Forms.Button();
            this.Table = new System.Windows.Forms.ListView();
            this.IdCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IdentCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InfoCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCode = new System.Windows.Forms.TabPage();
            this.tabPageId = new System.Windows.Forms.TabPage();
            this.tabPageLexeme = new System.Windows.Forms.TabPage();
            this.tableLexeme = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPageCode.SuspendLayout();
            this.tabPageId.SuspendLayout();
            this.tabPageLexeme.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnalizTextBox
            // 
            this.AnalizTextBox.Location = new System.Drawing.Point(8, 6);
            this.AnalizTextBox.Multiline = true;
            this.AnalizTextBox.Name = "AnalizTextBox";
            this.AnalizTextBox.Size = new System.Drawing.Size(450, 177);
            this.AnalizTextBox.TabIndex = 0;
            this.AnalizTextBox.Text = "z := 2*y^2+ x^3 - 2\r\nif z=2 then{\r\n x = 5;\r\n y = 2;\r\n}\r\nelse{\r\n x = 2;\r\n y = 5;\r\n" +
    "}\r\n";
            this.AnalizTextBox.TextChanged += new System.EventHandler(this.AnalizTextBox_TextChanged);
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.Location = new System.Drawing.Point(3, 3);
            this.ResultTextBox.MaximumSize = new System.Drawing.Size(4000, 4000);
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.ReadOnly = true;
            this.ResultTextBox.Size = new System.Drawing.Size(466, 20);
            this.ResultTextBox.TabIndex = 1;
            // 
            // AnalBut
            // 
            this.AnalBut.Location = new System.Drawing.Point(194, 189);
            this.AnalBut.Name = "AnalBut";
            this.AnalBut.Size = new System.Drawing.Size(75, 23);
            this.AnalBut.TabIndex = 2;
            this.AnalBut.Text = "Анализ";
            this.AnalBut.UseVisualStyleBackColor = true;
            this.AnalBut.Click += new System.EventHandler(this.AnalBut_Click);
            // 
            // Table
            // 
            this.Table.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IdCol,
            this.IdentCol,
            this.InfoCol});
            this.Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Table.FullRowSelect = true;
            this.Table.GridLines = true;
            this.Table.Location = new System.Drawing.Point(3, 3);
            this.Table.Name = "Table";
            this.Table.Size = new System.Drawing.Size(474, 338);
            this.Table.TabIndex = 3;
            this.Table.UseCompatibleStateImageBehavior = false;
            this.Table.View = System.Windows.Forms.View.Details;
            // 
            // IdCol
            // 
            this.IdCol.Text = "Id";
            // 
            // IdentCol
            // 
            this.IdentCol.Text = "Идентификатор";
            this.IdentCol.Width = 123;
            // 
            // InfoCol
            // 
            this.InfoCol.Text = "Информация";
            this.InfoCol.Width = 262;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Location = new System.Drawing.Point(8, 218);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(450, 101);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Ошибка";
            this.columnHeader1.Width = 376;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageCode);
            this.tabControl1.Controls.Add(this.tabPageId);
            this.tabControl1.Controls.Add(this.tabPageLexeme);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(488, 370);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPageCode
            // 
            this.tabPageCode.Controls.Add(this.AnalizTextBox);
            this.tabPageCode.Controls.Add(this.listView1);
            this.tabPageCode.Controls.Add(this.AnalBut);
            this.tabPageCode.Location = new System.Drawing.Point(4, 22);
            this.tabPageCode.Name = "tabPageCode";
            this.tabPageCode.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCode.Size = new System.Drawing.Size(480, 344);
            this.tabPageCode.TabIndex = 0;
            this.tabPageCode.Text = "Код";
            this.tabPageCode.UseVisualStyleBackColor = true;
            // 
            // tabPageId
            // 
            this.tabPageId.Controls.Add(this.Table);
            this.tabPageId.Location = new System.Drawing.Point(4, 22);
            this.tabPageId.Name = "tabPageId";
            this.tabPageId.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageId.Size = new System.Drawing.Size(480, 344);
            this.tabPageId.TabIndex = 1;
            this.tabPageId.Text = "Таблица идентификаторов";
            this.tabPageId.UseVisualStyleBackColor = true;
            // 
            // tabPageLexeme
            // 
            this.tabPageLexeme.Controls.Add(this.flowLayoutPanel1);
            this.tabPageLexeme.Location = new System.Drawing.Point(4, 22);
            this.tabPageLexeme.Name = "tabPageLexeme";
            this.tabPageLexeme.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLexeme.Size = new System.Drawing.Size(480, 344);
            this.tabPageLexeme.TabIndex = 2;
            this.tabPageLexeme.Text = "Таблица лексем";
            this.tabPageLexeme.UseVisualStyleBackColor = true;
            // 
            // tableLexeme
            // 
            this.tableLexeme.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.tableLexeme.FullRowSelect = true;
            this.tableLexeme.GridLines = true;
            this.tableLexeme.Location = new System.Drawing.Point(3, 29);
            this.tableLexeme.Name = "tableLexeme";
            this.tableLexeme.Size = new System.Drawing.Size(466, 304);
            this.tableLexeme.TabIndex = 4;
            this.tableLexeme.UseCompatibleStateImageBehavior = false;
            this.tableLexeme.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Лексема";
            this.columnHeader2.Width = 77;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Идентификатор";
            this.columnHeader3.Width = 123;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Информация";
            this.columnHeader4.Width = 192;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ResultTextBox);
            this.flowLayoutPanel1.Controls.Add(this.tableLexeme);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(474, 338);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 370);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPageCode.ResumeLayout(false);
            this.tabPageCode.PerformLayout();
            this.tabPageId.ResumeLayout(false);
            this.tabPageLexeme.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox AnalizTextBox;
        private System.Windows.Forms.TextBox ResultTextBox;
        private System.Windows.Forms.Button AnalBut;
        private System.Windows.Forms.ListView Table;
        private System.Windows.Forms.ColumnHeader IdCol;
        private System.Windows.Forms.ColumnHeader IdentCol;
        private System.Windows.Forms.ColumnHeader InfoCol;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCode;
        private System.Windows.Forms.TabPage tabPageId;
        private System.Windows.Forms.TabPage tabPageLexeme;
        private System.Windows.Forms.ListView tableLexeme;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}

