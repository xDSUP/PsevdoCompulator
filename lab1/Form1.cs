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

namespace lab1
{
  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void updateTableId(List<Lexeme> lexemes)
        {
            int newId = 0;
            Table.Items.Clear();
            foreach(Lexeme lexeme in lexemes)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = lexeme;
                item.Text = newId++.ToString();
                item.SubItems.Add(lexeme.Text);
                item.SubItems.Add(lexeme.type.ToString());
                Table.Items.Add(item);
            }
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

        void updateTables(Analizator analizator)
        {
            updateTableId(analizator.tableID);
            updateTableLexeme(analizator.tableLexemes, analizator.tableID);
        }

        private void AnalBut_Click(object sender, EventArgs e)
        {
            Analizator analizator = new Analizator(AnalizTextBox.Text);
            if (analizator.analysis()) // анализ успешно завершен
            {
                ResultTextBox.Text = analizator.ToString();
                updateTables(analizator);
            }
        }

        private void AnalizTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
