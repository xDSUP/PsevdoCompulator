using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using lab1.Syntax;

namespace lab1
{
  
    public partial class Form1 : Form
    {
        LexicalAnalizator analizator;
        SyntacticAnalizator syntacticAnalizator;
        public Form1()
        {
            InitializeComponent();
        }

        // void updateTableId(List<Lexeme> lexemes)
        void updateTableId(IdHashTable<Lexeme> idHashTable)
        {
            int newId = 0;
            Table.Items.Clear();
            var stack = idHashTable.Elems;
            foreach ( var id in stack)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = id;
                item.Text = 1.ToString();
                item.SubItems.Add(id.Lexeme.Text);
                Table.Items.Add(item);   
            }

            //foreach(Lexeme lexeme in lexemes)
            //{
            //    ListViewItem item = new ListViewItem();
            //    item.Tag = lexeme;
            //    item.Text = newId++.ToString();
            //    item.SubItems.Add(lexeme.Text);
            //    item.SubItems.Add(lexeme.type.ToString());
            //    Table.Items.Add(item);
            //}
        }

        void updateTableLexeme(List<Lexeme> lexemes, List<Lexeme>ids)
        {
            tableLexeme.Items.Clear();
            foreach (Lexeme lexeme in lexemes)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = lexeme;
                item.Text = lexeme.Text;
                if(lexeme.type == Lexeme.LexemType.CONSTANT ||
                    lexeme.type == Lexeme.LexemType.CONSTANT_DOUBLE ||
                    lexeme.type == Lexeme.LexemType.ID)
                {
                    item.SubItems.Add(ids.IndexOf(lexeme).ToString());
                }
                else
                {
                    item.SubItems.Add("-");
                }
                item.SubItems.Add(lexeme.type.ToString());
                tableLexeme.Items.Add(item);
            }
        }

        void updateTables(LexicalAnalizator analizator)
        {
            updateTableId(analizator.idHashTable);
            updateTableLexeme(analizator.tableLexemes, analizator.tableID);
        }

        private void AnalBut_Click(object sender, EventArgs e)
        {
            LexicalAnalizator analizator = new LexicalAnalizator(AnalizTextBox.Text);
            if (analizator.analysis()) // анализ успешно завершен
            {
                ResultTextBox.Text = analizator.ToString();
                this.analizator = analizator;
                updateTables(analizator);
            }
            syntacticAnalizator = new SyntacticAnalizator(analizator.tableLexemes);
            try
            {
                syntacticAnalizator.work();
                UpdateTreeView(syntacticAnalizator.listExpression);
            }
            catch (lab1.Exceptions.SyntaxException exp)
            {
                MessageBox.Show(exp.Message);
            }
  
        }
        
        private void UpdateTreeView(List<Expression> expressions)
        {
            treeView1.Nodes.Clear();
            foreach(var expression in expressions)
            {
                var node = new TreeNode();
                node.Tag = expression;
                node.Text = expression.ToString();
                treeView1.Nodes.Add(node);
            }
        }

        private void AnalizTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
