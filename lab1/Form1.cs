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
using lab1.CodeGenerate;

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
                item.SubItems.Add(id.Value.ToString());
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
                updateTableLexeme(analizator.tableLexemes, analizator.tableID);
            }
            syntacticAnalizator = new SyntacticAnalizator(analizator.tableLexemes);
            try
            {
                syntacticAnalizator.work();
                UpdateTreeView(syntacticAnalizator.listExpression);
                MessageBox.Show("Построение завершено");
                CodeGenerator codeGenerator = new CodeGenerator(syntacticAnalizator.listExpression, analizator.idHashTable);
                codeGenerator.Generate();
                updateTableId(analizator.idHashTable);
                UpdateCodeView(codeGenerator.codeBlocks);
                CodeOptimizator optimizator = new CodeOptimizator(codeGenerator.codeBlocks, analizator.idHashTable);
                optimizator.Optimize();
                UpdateCodeOptView(codeGenerator.codeBlocks);
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
                treeView1.Nodes.Add(getTreeNode(expression));
            }
        }

        private TreeNode getTreeNode(Object obj)
        {
            TreeNode temp = new TreeNode();
            if(obj is null)
            {
                temp.Text = "NULL";
            }
            else if (obj is Lexeme)
            {
                Lexeme lexeme = obj as Lexeme;
                temp.Tag = lexeme;
                temp.Text = lexeme.Text;
            }
            else if (obj is Expression)
            {
                Expression exp = obj as Expression;
                temp.Tag = exp;
                temp.Text = exp.ToString() + exp.state;
                temp.Nodes.Add(getTreeNode(exp.Left));
                temp.Nodes.Add(getTreeNode(exp.Oper));
                temp.Nodes.Add(getTreeNode(exp.Right));
            }
            else
            {
                // лист
                List<Expression> list = obj as List<Expression>;
                temp.Text = obj.ToString();
                foreach (var item in list)
                {
                    temp.Nodes.Add(getTreeNode(item));
                }
            }
            return temp;
        }

        private TreeNode getTreeNode(CodeBlock block)
        {
            TreeNode temp = new TreeNode();
            foreach(var oper in block.operations)
            {
                temp.Tag = block;
                temp.Text = block.expression.ToString();
                temp.Nodes.Add(oper.ToString());
            }

            return temp;
        }

        private void AnalizTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateCodeView(List<CodeBlock> codeBlocks)
        {
            treeView2.Nodes.Clear();
            
            foreach (var block in codeBlocks)
            {
                treeView2.Nodes.Add(getTreeNode(block));
            }
        }

        private void UpdateCodeOptView(List<CodeBlock> codeBlocks)
        {
            treeView3.Nodes.Clear();

            foreach (var block in codeBlocks)
            {
                treeView3.Nodes.Add(getTreeNode(block));
            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
