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
            return $"{s} {l?.ToString()}";
        }
    }
    // блок который генерируется для выражений
    public class CodeBlock
    {
        int _id = 0;
        public int Id { get => _id++; }

        public string getNewId() => "VAR" + Id;
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
                currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, l));
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
                currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, l));
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
            currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, tempL.s));
            //stack.Push(tempR); // кидаем левую в стек, по ёё имени и сохраним результат
            // вынужденная мера, чтобы не переписывать хештаблицу(((
            var lexL = new Lexeme(tempL.s, Lexeme.LexemType.ID);
            var lexR = new Lexeme(tempR.s, Lexeme.LexemType.ID);
            switch (exp.state)
            {
                case State.OPER_MPY:
                    hashTable.lookUp(lexL).Value *= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.MPY, lexR));
                    break;
                case State.OPER_DIV:
                    hashTable.lookUp(lexL).Value /= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.DIV, lexR));
                    break;
                case State.OPER_MINUS:
                    hashTable.lookUp(lexL).Value -= hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.SUB, lexR));
                    break;
                case State.OPER_PLUS:
                    hashTable.lookUp(lexL).Value += hashTable.lookUp(lexR).Value;
                    currentOperations.Add(new CodeOperation(CodeOperationType.ADD, lexR));
                    break;
                case State.OPER_LOAD:
                    // выкинем из стека все)
                    stack.Pop();
                    currentOperations.Add(new CodeOperation(CodeOperationType.LOAD, lexR));
                    // сохраним рузельтат
                    currentOperations.Add(new CodeOperation(CodeOperationType.STORE, tempL.l));
                    // заносим значение в таблицу
                    hashTable.lookUp(tempL.l).Value = hashTable.lookUp(lexR).Value;
                    break;
            }
            // закинем результат в переменную
            if(exp.state != State.OPER_LOAD)
                currentOperations.Add(new CodeOperation(CodeOperationType.STORE, stack.Peek().s));
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
                operations = genByIfThen(expression);
                operations.Add(new CodeOperation(CodeOperationType.POINT, stack.Pop().s));
            }
            else if (expression.state == State.IFTHENELSE)
            {
                operations = genByIfThenElse(expression);
            }
            else
            {
                operations = genByExp(expression);
            }


        }

        private List<CodeOperation> genByIfThenElse(Expression expression)
        {
            var opers = genByIfThen(expression);
            genByListExpr(expression.Right, opers);
            opers.Add(new CodeOperation(CodeOperationType.POINT, stack.Pop().s));
            return opers;
        }

        private List<CodeOperation> genByIfThen(Expression expression)
        {
            var opers = new List<CodeOperation>();
            // сначала обработаем условие

            var condition = expression.getLeftAsExpr();
            opers.Add(new CodeOperation(CodeOperationType.LOAD, condition.Left));
            var operatorCond = condition.Oper as Lexeme;
            switch (operatorCond.Text)
            {
                case "=":
                    opers.Add(new CodeOperation(CodeOperationType.EQUAL, condition.Right));
                    break;
                case "<":
                    opers.Add(new CodeOperation(CodeOperationType.LT, condition.Right));
                    break;
                case ">":
                    opers.Add(new CodeOperation(CodeOperationType.GT, condition.Right));
                    break;
            }
            // метка
            string point = ":pointEndThen" + getNewId();
            //закинем в стек метку для 
            //stack.Push(new tempId(null, point));
            opers.Add(new CodeOperation(CodeOperationType.JZ, point));
            // пишем код для then

            genByListExpr(expression.Oper, opers);
            string pointElse = ":pointEndElse" + getNewId();
            //закинем в стек метку для елсе
            stack.Push(new tempId(null, pointElse));
            // переход за пределы елсе
            opers.Add(new CodeOperation(CodeOperationType.JMP, pointElse));
            opers.Add(new CodeOperation(CodeOperationType.POINT, point));
            return opers;
        }

        // exp - либо лист либо 1 выражение
        private void genByListExpr(object expression, List<CodeOperation> opers)
        {
            var exp = expression as Expression;
            // если внутри несколько выражений 
            if (exp.Oper is List<Expression>)
            {
                foreach (var expr in (List<Expression>)exp.Oper)
                {
                    opers.AddRange(genByExp(expr));
                }
            }
            else // значит внутри только одно
            {
                opers.AddRange(genByExp((Expression)exp.Oper));
            }
        }
    }
}
