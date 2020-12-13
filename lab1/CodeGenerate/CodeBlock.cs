using lab1.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.CodeGenerate
{
    struct tempId
    {
        public Lexeme l;
        public string s;

        public tempId(Lexeme l, string s)
        {
            this.l = l;
            this.s = s;
        }

        public override string ToString()
        {
            return $"{s} {l}";
        }
    }
    // блок который генерируется для выражений
    public class CodeBlock
    {
        int _id = 0;
        public int Id { get => _id++; }

        public string getNewId() => "$" + Id;
        // выражение, для которого генерируем код
        public Expression expression;
        /// <summary>
        /// лист кода на псевдо-асемблере
        /// </summary>
        public List<CodeOperation> operations;
        IdHashTable<Lexeme> hashTable;
        /// <summary>
        /// для хранения значений store
        /// </summary>
        Stack<tempId> stack;

        public CodeBlock(Expression expression, IdHashTable<Lexeme> hashTable)
        {
            this.hashTable = hashTable;
            operations = new List<CodeOperation>();
            stack = new Stack<tempId>();
            this.expression = expression;
        }

        private List<CodeOperation> genByExp(Expression exp)
        {
            List<CodeOperation> currentOperations = new List<CodeOperation>();
            var left = exp.getLeftAsExpr();
            var right = exp.getRightAsExpr();
            Lexeme l;
            
            // сначала грузим все по порядку
            if (left != null)
            {
                currentOperations.AddRange(genByExp(left));
            }
            else // если слева лексема 
            {
                // загружаем и всегда кидаем в стек
                l = (Lexeme)exp.Left;
                currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, hashTable.lookUp(l).Value));
                string t = getNewId();
                currentOperations.Add(new CodeOperation(CodeOperationType.STORE, t));
                // попробуем поддрживать нужные значения в таблице хеша
                var lex = new Lexeme(t, Lexeme.LexemType.ID);
                var tem = hashTable.lookUp(lex);
                // если такого айди не было, вставим
                if (tem == null)
                    hashTable.insert(lex);
                hashTable.lookUp(lex).Value = hashTable.lookUp(l).Value;
                stack.Push(new tempId(l, t));
                //hashTable.insert(t);
            }
            if (right != null)
            {
                currentOperations.AddRange(genByExp(right));
            }
            else
            {
                // загружаем и всегда кидаем в стек
                l = (Lexeme)exp.Right;
                currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, hashTable.lookUp(l).Value));
                string t = getNewId();
                currentOperations.Add(new CodeOperation(CodeOperationType.STORE, t));

                var lex = new Lexeme(t, Lexeme.LexemType.ID);
                var tem = hashTable.lookUp(lex);
                // если такого айди не было, вставим
                if (tem == null)
                    hashTable.insert(lex);
                hashTable.lookUp(lex).Value = hashTable.lookUp(l).Value;
                stack.Push(new tempId(l, t));
            }
            // загрузим правую и левую часть операции
            var tempR = stack.Pop();
            var tempL = stack.Peek();
            // в сумматор кладем левую часть
            currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, tempL));
            //stack.Push(tempR); // кидаем левую в стек, по ёё имени и сохраним результат
            // вынужденная мера, чтобы не переписывать хештаблицу(((
            var lexL = new Lexeme(tempL.s, Lexeme.LexemType.ID);
            var lexR = new Lexeme(tempR.s, Lexeme.LexemType.ID);
            switch (exp.state)
            {
                case State.OPER_MPY:
                    hashTable.lookUp(lexL).Value *= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.MPY, stack.Peek()));
                    break;
                case State.OPER_DIV:
                    hashTable.lookUp(lexL).Value /= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.DIV, stack.Peek()));
                    break;
                case State.OPER_MINUS:
                    hashTable.lookUp(lexL).Value -= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.SUB, stack.Peek()));
                    break;
                case State.OPER_PLUS:
                    hashTable.lookUp(lexL).Value += hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.ADD, stack.Peek()));
                    break;
                case State.OPER_LOAD:
                    // выкинем из стека все)
                    stack.Pop();
                    currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, tempR.l));
                    // сохраним рузельтат
                    currentOperations.Add(new CodeOperation(CodeOperationType.STORE, tempL.l));
                    // заносим значение в таблицу
                    hashTable.lookUp(tempL.l).Value = hashTable.lookUp(lexR).Value;
                    break;
            }
            // закинем результат в переменную
            if(exp.state != State.OPER_LOAD)
                currentOperations.Add(new CodeOperation(CodeOperationType.STORE, stack.Peek()));
            return currentOperations;
        }

        /// <summary>
        /// генерирует код для текущего блока кода
        /// </summary>
        public void generate()
        {

            // отдельно обрабатываем код для ифов
            if (expression.state == State.IFTHEN)
            {
                
            }
            else if (expression.state == State.IFTHENELSE)
            {

            }
            else
            {
                operations = genByExp(expression);
            }


        }
    }
}
