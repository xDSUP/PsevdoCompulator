using lab1.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1.Syntax
{
    /// <summary>
    /// интерфейс правила с единственным методом
    /// который проверяет выражение на соответствие правилу
    /// </summary>
    public interface IRule
    {
        bool accept(Expression exp);
    }

    public enum State
    {
        NOT_OPERATOR,
        NOT_RIGHT,
        NOT_LEFT,
        OK,
        IF,
        IFTHEN,
        IFTHENELSE,
        ISBRACES, // фигурные скобочки {}
        ISBRAKETS // обычные скобочки ()
    }
    /// <summary>
    /// Выражение состоит из 2 - х лексем|выражений и оператора
    /// это могут быть (|{ выражение )|}
    /// конструкция ifthen, где левое дерево это выражение условия, а среднее это выражение после then
    /// конструкция ifthenelse левая и средняя аналогично ifthen, а правое - выражение else
    /// важное условие для граматики! : выражения после then and else всегда в {}!
    /// </summary>
    public class Expression
    {
        private object left; // левая часть выражения
        private object oper; // оператор либо средняя часть выражения
        private object right; // правая часть выражения 
        public State state; // состояние выражения
        // флаг проверено или нет, чтобы два раза не проверять при вложенности
        private bool isChecked = false; 

        public object Left { 
            get => left; 
            set
            {
                left = value;
            }
        }
        public object Oper
        {
            get => oper;
            set
            {
                oper = value;
            }
        }
        public object Right
        {
            get => right;
            set
            {
                right = value;
            }
        }

        // подошедшее правило
        private Func<Expression, bool> trueRule;

        public Expression()
        {
             state = State.NOT_RIGHT;
        }

        public Expression(object obj)
        {
            Left = obj;
            state = State.NOT_OPERATOR;
        }

        public Expression(object left, Lexeme oper, object right)
        {
            this.Left = left;
            this.Oper = oper;
            this.Right = right;
            state = State.OK;
        }

        #region Проверки на лексемы и выражения
        private bool _lIsLex() => left is Lexeme;
        private bool _rIsLex() => right is Lexeme;
        private bool _oIsLex() => oper is Lexeme;
        private bool _lIsExp() => left is Expression;
        private bool _rIsExp() => right is Expression;
        private bool _oIsExp() => oper is Expression;
        private bool _lIsLexORExp() => _lIsExp() || _lIsLex();
        private bool _rIsLexORExp() => _rIsExp() || _rIsLex();
        private bool _oIsLexORExp() => _oIsExp() || _oIsLex();
        private bool _lStateIs(State st) => ((Expression)left).isTrue() && ((Expression)left).state==st;
        private bool _rStateIs(State st) => ((Expression)right).isTrue() && ((Expression)right).state == st;
        private bool _oStateIs(State st) => ((Expression)oper).isTrue() && ((Expression)oper).state == st;
        #endregion

        #region Сборник правил
        Func<Expression, bool>[] rules =
        {
             // проверка обычной арифметической операции
             (e) => e._lIsLexORExp() && e._oIsLex() && e._rIsLexORExp(),
             // проверка выражения скобочек для ифа
             (e) => e._lIsLex() && e.left.ToString()=="{" && e._oIsLexORExp() 
             && e._rIsLexORExp() && e.left.ToString()=="}",
             // проверка выражения скобочек
             (e) => e._lIsLex() && e.left.ToString()=="(" && e._oIsLexORExp() 
             && e._rIsLexORExp() && e.left.ToString()==")",
             // проверка ифа
             (e) => e._lIsExp() && e._oIsExp()
        };
        //IRule[] rules =
        #endregion

        public bool isTrue()
        {
            if (isChecked)
                return true;
            // пройдем по всем правилам, если хоть одно подойдет
            // все оки, иначе это ошибка
            //foreach (var rule in rules)
            //{
            //    if (rule.Invoke(this))
            //    {
            //        trueRule = rule;
            //        isChecked = true;
            //        return true;
            //    }
            //}
            //return false;

            // проверка левого и правого операнда, если там скобочки, отметит соотв-щим образом флагами 
            // ISDRACES b ISBRAKETS соответственно 
            if (!CheckOperand(Left))
                throw new SyntaxException($"Слева в {this} что-то не то");

            if (!CheckOperand(Right))
                throw new SyntaxException($"Справа в {this} что-то не то");

            // проверка среднего операнда 
            if(oper is Lexeme)
            {
                // если это лексема, значит это обычная операция, либо 1 операнд внутри скобок!
                var tempOper = Oper as Lexeme;
                if (tempOper.type != Lexeme.LexemType.OPERATOR)
                {
                    throw new SyntaxException("Это " + tempOper.Text + " не оператор!");
                }
                else if (tempOper.isConst())
                {
                    // если в середине константа, значит по бокам скобочки, иначе кидаем ошибочку
                    if (state != State.ISBRACES && state != State.ISBRAKETS)
                        throw new SyntaxException("Константа в середине????");

                }
            }
            else if(oper is Expression) // если по середине выражение, значит это должны быть скобочки, либо иф
            {
                if(state != State.IFTHEN && state != State.IFTHENELSE && state != State.ISBRACES && state != State.ISBRAKETS)
                {
                    throw new SyntaxException($"Не понимаю зачем  {this}здесь выражение");
                }
                var expr = oper as Expression;
                if(expr.left != null)
                return expr.isTrue();
            }
            return isChecked = true;
        }

        /// <summary>
        ///  вернет пустой или нет
        /// </summary>
        /// <returns></returns>
        public bool isNull()
        {
            return right is null && oper is null && left is null;
        }

        private bool CheckOperand(object obj)
        {
            if (obj is Lexeme)
            {
                var lexeme = (Lexeme)obj;
                // если это не константа или айди
                if((((Lexeme) obj).type == Lexeme.LexemType.CONSTANT || 
                    ((Lexeme)obj).type == Lexeme.LexemType.CONSTANT_DOUBLE || 
                    ((Lexeme)obj).type == Lexeme.LexemType.ID))
                {
                    return true;
                }
                // если это разделитель
                else if(lexeme.type == Lexeme.LexemType.LIMITERS)
                {
                    switch (lexeme.Text)
                    {
                        case "{":
                            state = State.ISBRACES;
                            break;
                        case "}":
                            if(state != State.ISBRACES)
                            {
                                throw new SyntaxException($"Обнаружена скобка {this}");
                            }
                            break;
                        case "(":
                            state = State.ISBRAKETS;
                            break;
                        case ")":
                            if (state != State.ISBRAKETS)
                            {
                                throw new SyntaxException($"Обнаружена скобка {this}");
                            }
                            break;
                        default:
                            throw new SyntaxException($"Обнаружен символ на непонятном месте {this}");
                            return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if(obj is Expression)
            {
                return ((Expression)obj).isTrue();
            }
            throw new SyntaxException("Что-то неизвестное внутри");
        }

        public override string ToString()
        {
            String operStr = oper is List<Expression> 
                ? String.Join(
                    "; ", ((List<Expression>)oper).ConvertAll(new Converter<Expression, String>(x=>x.ToString()))) 
                : oper?.ToString();
            return $"{left?.ToString()} {operStr?.ToString()} {right?.ToString()}";
        }
    }
}
