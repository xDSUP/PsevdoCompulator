using lab1.Exceptions;
using lab1.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1
{
    /// <summary>
    /// Класс синтаксического анализатора, который анализирует список лексем на соответствие правилам языка
    /// </summary>
    class SyntacticAnalizator
    {
        // список лексем
        private List<Lexeme> lexemes;
        public List<Expression> listExpression;
        /// <summary>
        /// Тут будем хранить последовательность блоков из лексем
        /// блок представляет из себя дерево
        /// </summary>

        /// <summary>
        /// счетчик скобочек: открывающие делают +1, закрывающие -1
        /// в конце разбора должен быть 0, а в процессе число не может быть отрицательным
        /// </summary>
        private int _limitersCounter = 0;

        public SyntacticAnalizator(List<Lexeme> lexemes)
        {
            this.lexemes = lexemes;
        }

        // запускает анализ списка лексем
        public void work()
        {
            listExpression = new List<Expression>(); // для хранение узлов, чтобы спускать вниз и возвращаться
            // для хранение листов выражений
            Stack<List<Expression>> listsExpressions = new Stack<List<Expression>>();
            listsExpressions.Push(listExpression); // самый главный лист всегда в стеке!!!
            // для хранения родителей при погружении в скобочки и if'ы
            Stack<Expression> stackExpression = new Stack<Expression>(); 
            Lexeme tempLexeme;
            Expression currentExpression = null;
            Expression tempExpression = null;
            
            // выполняется проход по каждой лексеме для составления дерева разбора
            for (int cur = 0; cur < lexemes.Count; cur++)
            {
                var currentLexeme = lexemes[cur];
                if (tempExpression == null)
                {
                    tempExpression = new Expression();
                    currentExpression = tempExpression;
                }

                if (currentLexeme.type == Lexeme.LexemType.CONSTANT ||
                    currentLexeme.type == Lexeme.LexemType.ID ||
                    currentLexeme.type == Lexeme.LexemType.CONSTANT_DOUBLE)
                {
                    if(currentExpression.Left is null)
                    {
                        currentExpression.Left = currentLexeme;
                    }                    
                    else if (currentExpression.Right is null) // правого нет
                    {
                        currentExpression.Right = currentLexeme;
                    }
                    else // правый уже есть
                    {
                        throw new SyntaxException("Две константы рядом");
                    } 
                }

                if (currentLexeme.type == Lexeme.LexemType.LIMITERS)
                {
                    switch (currentLexeme.Text)
                    {
                        case "{": // значит начинается новый блок кода
                            _limitersCounter++;
                            if (!currentExpression.isNull())
                            {
                                throw new SyntaxException("Обнаруженно незаконченное выражение: " + tempExpression.ToString());
                            }
                            else
                            {
                                currentExpression.Left = currentLexeme;
                                var temp = new Expression();
                                stackExpression.Push(currentExpression);
                                tempExpression = temp;
                                currentExpression = temp;
                                listsExpressions.Push(new List<Expression>());
                            }
                            break;
                        case "}": // конец блока кода
                            _limitersCounter--;
                            if (_limitersCounter < 0)
                            {
                                throw new SyntaxException("Есть лишние закрывающие скобочки");
                            }
                            currentExpression = stackExpression.Pop();
                            if (currentExpression.Right is null)
                            {
                                currentExpression.Right = currentLexeme;
                            }
                            currentExpression.Oper = listsExpressions.Pop();
                            currentExpression = stackExpression.Pop();
                            tempExpression = currentExpression;
                            break;
                        case "(":
                            _limitersCounter+= 2;
                            if (tempExpression.isNull())
                            {
                                throw new SyntaxException("Скобка в пустом выражении" + tempExpression.ToString());
                            }
                            else
                            {
                                var temp = new Expression();
                                // если слева пустое место
                                if (currentExpression.Left == null)
                                {
                                    // вроде не долнжо сюда заходить
                                    currentExpression.Left = currentLexeme;
                                    throw new SyntaxException("Скобочка слева!!! " + tempExpression);
                                }
                                else // что-то есть
                                {
                                    if (currentExpression.Oper == null)
                                        throw new SyntaxException("Перед скобочкой нет операции " + tempExpression);
                                    // сохраним вложенность правильно
                                    stackExpression.Push(tempExpression);
                                    stackExpression.Push(currentExpression); 
                                    
                                    // теперь справа будет выражение в скобочках
                                    currentExpression.Right = new Expression() { Left = currentLexeme, Oper = temp };
                                    
                                    stackExpression.Push((Expression)currentExpression.Right);
                                    // погружаемся внутрь этих скобочек
                                    tempExpression = temp;
                                    currentExpression = temp;
                                    listsExpressions.Push(new List<Expression>());
                                }
                            }
                            break;
                        case ")":
                            _limitersCounter-= 2;
                            if (_limitersCounter < 0)
                            {
                                throw new SyntaxException("Есть лишние закрывающие скобочки");
                            }
                            // выражение в скобочках кешируем для проверки
                            var tempExpr = currentExpression;
                            // получаем выражение со скобочками
                            currentExpression = stackExpression.Pop();
                            if (currentExpression.Right is null) 
                            {
                                currentExpression.Right = currentLexeme;
                            }
                            // получаем список выражений, которые фиксировались до этой скобочки

                            var listExpr = listsExpressions.Pop();
                            if (listExpr.Count >= 1)
                            {
                                currentExpression.Oper = listExpr;
                            }
                            else
                            {
                                // проверим выражение внутри скобочек на правильность(для ожного элемента)
                                if(tempExpr.Left is Lexeme && tempExpr.Oper is null && tempExpr.Right is null )
                                    currentExpression.Oper = tempExpr.Left;
                                else
                                {
                                    if (!tempExpr.isTrue())
                                    {
                                        throw new SyntaxException("Неправильное выражение в скобках " + tempExpr);
                                    }
                                    else
                                    {
                                        currentExpression.Oper = tempExpr;
                                    }
                                }
                            }
                                
                            //else этот случай уже обработан изначально
                                //currentExpression.Oper = null;
                            // вернем контекст, который был до скобочек
                            currentExpression = stackExpression.Pop();
                            tempExpression = stackExpression.Pop();
                            break;
                        case ";":
                            try
                            {
                                if (!tempExpression.isTrue())
                                {
                                    throw new SyntaxException("Ошибка в выражении " + tempExpression);
                                }
                            }
                            catch (SyntaxException exp)
                            {
                                throw new SyntaxException(exp.Message + "\nОшибка в выражении " + tempExpression);
                            }
                            
                            listsExpressions.Peek().Add(tempExpression);
                            tempExpression = null; // всё оки
                            break;
                        default:
                            throw new SyntaxException("Непонятный символ: " + currentLexeme);
                    }
                }
                if(currentLexeme.type == Lexeme.LexemType.OPERATOR)
                {
                    if (currentExpression.Oper is null)
                    {
                        currentExpression.Oper = currentLexeme;
                    }
                    else
                    {
                        var temp = new Expression(currentExpression.Right);
                        temp.Oper = currentLexeme;
                        currentExpression.Right = temp;
                        currentExpression = temp;
                    }
                }
                if (currentLexeme.type == Lexeme.LexemType.KEY_WORD)
                {
                    if (currentLexeme.Text == "if")
                    {
                        if (!currentExpression.isNull())
                        {
                            throw new SyntaxException("Обнаруженно незаконченное выражение: " + tempExpression.ToString());
                        }
                        else
                        {
                            var temp = new Expression();
                            currentExpression.Left = temp;
                            stackExpression.Push(currentExpression); // погрузимся на уровень вниз для выражения там
                            currentExpression = temp;
                        }
                    }
                    if (currentLexeme.Text == "then")
                    {
                        // значит раньше было вложенное условие и теперь пора подниматься на уровень с ифом
                        currentExpression = stackExpression.Peek();
                        currentExpression.state = State.IFTHEN;
                        var temp = new Expression();
                        currentExpression.Oper = temp;
                        currentExpression = temp;
                    }
                    if (currentLexeme.Text == "else")
                    {
                        currentExpression.state = State.IFTHENELSE;
                        stackExpression.Push(currentExpression);
                        var temp = new Expression();
                        currentExpression.Right = temp;
                        currentExpression = temp;
                    }
                }
                tempLexeme = currentLexeme;
            }
            if(tempExpression != null)
            {
                throw new SyntaxException($"Есть незакоченное выражение : {tempExpression}");
            }
            if(_limitersCounter != 0)
            {
                throw new SyntaxException($"Есть незакрытые скобочки!");
            }
        }
    }
}
