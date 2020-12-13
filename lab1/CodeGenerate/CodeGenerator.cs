using lab1.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.CodeGenerate
{
    public class CodeGenerator
    {
        
        // дерево выражений
        private List<Expression> expressions;
        // для каждого выражения соотв дерево кода
        public List<CodeBlock> codeBlocks;
        IdHashTable<Lexeme> hashTable;

        public CodeGenerator(List<Expression> expressions, IdHashTable<Lexeme> hashTable)
        {
            this.hashTable = hashTable;
            this.expressions = expressions;
            codeBlocks = new List<CodeBlock>();
        }

        // генерирует код для дерева
        public void Generate()
        {
            foreach (var exp in expressions)
            {
                var c = new CodeBlock(exp, hashTable);
                c.generate();
                codeBlocks.Add(c);
            }
        }
    }
}
