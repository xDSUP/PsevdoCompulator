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
            // предыдущая лексема
            Lexeme tempLexeme;
            // хранит текущую ветку выражения
            Expression currentExpression = null;
            // хранит полное выражение
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

                if (currentLexeme.isConst())
                {
                    // если есть опер и нет правого, добавим правый, это для унарного минуса
                    if(currentExpression.Oper is not null && currentExpression.Right is null)
                    {
                        currentExpression.Right = currentLexeme;
                    }
                    else if(currentExpression.Left is null)
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
                        case "{": // значит начинается новый блок кода в ифах
                            _limitersCounter++;
                            if (!currentExpression.isNull()) // текущее должно быть пустым
                            {
                                throw new SyntaxException("Обнаруженно незаконченное выражение: " + tempExpression.ToString());
                            }
                            else if (stackExpression.Count == 0 || (stackExpression.Peek().state != State.IFTHEN && stackExpression.Peek().state != State.IFTHENELSE))
                            {
                                // если скобочка встретилась не после выражения ифа, значит плохо
                                throw new SyntaxException("Фигурная скобка в недопустимом месте " + tempExpression);
                            }
                            else
                            {
                                // теперь это выражение со скобками
                                currentExpression.state = State.ISBRAKETS;
                                currentExpression.Left = currentLexeme;
                                stackExpression.Push(currentExpression); // чтобы вернуться к выражению со скобкой

                                var tempE = new Expression(); // для записи выражений внутри скобок
                                tempExpression = tempE;
                                currentExpression = tempE;
                                // выражения по ; будут записываться в лист, пока не встретим } , а потом вернемся к обычному листу(см ниже)
                                listsExpressions.Push(new List<Expression>());
                            }
                            break;
                        case "}": // конец блока кода
                            _limitersCounter--;
                            if (_limitersCounter < 0)
                            {
                                throw new SyntaxException("Есть лишние закрывающие скобочки " + tempExpression);
                            }
                            else if (stackExpression.Count == 0)
                            {
                                // не может быть пустым стек при скобочку, значит что-то не так
                                throw new SyntaxException("Есть лишние закрывающие скобочки " + tempExpression);
                            }
                            // получаем выражение со скобочками
                            currentExpression = stackExpression.Pop();
                            if (currentExpression.state != State.ISBRAKETS)
                                throw new SyntaxException("Есть лишние закрывающие скобочки " + tempExpression);

                            if (currentExpression.Right is null) // правого не должно быть
                            {
                                currentExpression.Right = currentLexeme;
                            }
                            // подставим ближайший лист выражений в наши скобки
                            var tempList = listsExpressions.Pop();
                            // если выражение одно, то запишем его без листа
                            if (tempList.Count == 0)
                                throw new SyntaxException("Внутри скобок нет выражений" + tempExpression);
                            currentExpression.Oper = tempList.Count == 1 ? (object)tempList[0] : (object)tempList;
                            // достаем выражение ифа
                            currentExpression = stackExpression.Pop();
                            // устанавливаем его текущим
                            tempExpression = currentExpression;
                            break;
                        case "(":
                            _limitersCounter += 2;

                            var temp = new Expression();
                            // сохраним вложенность правильно
                            stackExpression.Push(tempExpression); // полное выражение где встретились скобочки
                            stackExpression.Push(currentExpression); // выражение, где непосредств встретились скобочки
                                                                     // для случаев сразу скобок в скобках
                            if (currentExpression.isNull())
                            {
                                // теперь слева будет выражение в скобочках
                                currentExpression.Left = new Expression() { Left = currentLexeme, Oper = temp, state = State.ISBRACES };
                                // помещаем в стек выражение со скобками
                                stackExpression.Push((Expression)currentExpression.Left);
                                // для этого случая ссылка на выраж со скобкой уже в стеке
                            }
                            // если в текущем выражении еще нет операции и есть что-то слева
                            else if (currentExpression.Oper == null)
                            {
                                throw new SyntaxException("Перед скобочкой нет операции " + tempExpression);
                            }
                            else if (currentExpression.Left is not null && currentExpression.Oper is not null)
                            {
                                // теперь справа будет выражение в скобочках
                                currentExpression.Right = new Expression() { Left = currentLexeme, Oper = temp, state = State.ISBRACES };
                                // помещаем в стек выражение со скобками
                                stackExpression.Push((Expression)currentExpression.Right);
                            }
                            else
                            {
                                throw new SyntaxException("Не понимаю, как толковать " + tempExpression);
                            }
                            // погружаемся внутрь этих скобочек
                            tempExpression = temp;
                            currentExpression = temp;
                            // в скобках не будет ;, а значит лист излишен
                            //listsExpressions.Push(new List<Expression>());

                            break;
                        case ")":
                            _limitersCounter -= 2;
                            if (_limitersCounter < 0)
                            {
                                throw new SyntaxException("Есть лишние закрывающие скобочки");
                            }
                            // выражение в скобочках кешируем для проверки
                            var tempExpr = currentExpression;
                            // получаем выражение со скобочками
                            currentExpression = stackExpression.Pop();
                            if (currentExpression.state != State.ISBRACES)
                                throw new SyntaxException("Скобка закрывает что-то не то " + tempExpression);

                            currentExpression.Right = currentLexeme;// ставим скобку на место
                            // проверим выражение внутри скобочек на правильность(для кажного элемента)
                            if (tempExpr.Left is Lexeme && tempExpr.Oper is null && tempExpr.Right is null) // небольшое сокращение
                            {
                                currentExpression.Oper = tempExpr.Left;
                            }
                            else
                            {
                                if (!tempExpression.isTrue()) // проверка выражения в скобках
                                {
                                    throw new SyntaxException("Неправильное выражение в скобках " + tempExpression);
                                }
                                else
                                {
                                    currentExpression.Oper = tempExpression;
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
                        // до ифа ничего не законченнго не может быть!
                        if (!currentExpression.isNull())
                        {
                            throw new SyntaxException("Обнаруженно незаконченное выражение: " + tempExpression.ToString());
                        }
                        else
                        {
                            currentExpression.state = State.IF;
                            var temp = new Expression();
                            currentExpression.Left = temp;
                            stackExpression.Push(currentExpression); // сохраним ссылку на выражение ифа
                            // погрузимся на уровень вниз для записи условия в левый узел выражения ифа
                            currentExpression = temp;
                        }
                    }
                    if (currentLexeme.Text == "then")
                    {
                        // значит раньше было вложенное условие и теперь пора подниматься на уровень выражения с ифом
                        currentExpression = stackExpression.Peek();
                        if (currentExpression.state != State.IF)
                            throw new SyntaxException("then не внутри конструкции ifthen " + currentExpression);
                        // логическое выражение уже записано для ифа
                        currentExpression.state = State.IFTHEN;
                        var temp = new Expression();
                        // теперь спускаемся для записи выражений внутри then
                        currentExpression.Oper = temp;
                        currentExpression = temp;
                        tempExpression = currentExpression;
                    }
                    if (currentLexeme.Text == "else")
                    {
                        // после then была {} и мы поднялись на уровень с ифом
                        // значит раньше был then
                        if(currentExpression.state != State.IFTHEN)
                            throw new SyntaxException("else не внутри конструкции ifthenelse " + currentExpression);
                        currentExpression.state = State.IFTHENELSE;
                        stackExpression.Push(currentExpression); // записываем в стек, чтобы вернуться к выражению ифа
                        var temp = new Expression();
                        currentExpression.Right = temp;
                        // опускаемся для записи выражений в else
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
