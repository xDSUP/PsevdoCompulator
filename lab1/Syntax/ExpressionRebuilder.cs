using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.Syntax
{
    
    class ExpressionRebuilder
    {
        // выражение которое перестраиваем
        public Expression expression;

        // стек где, будем хранить уровень вложенности
        private Stack<Expression> stack = new Stack<Expression>();

        public ExpressionRebuilder(Expression expression)
        {
            this.expression = expression;
        }

        public void Rebuild()
        {
            // пока что-то меняется)
            while (checkExpression(expression)) ;
        }

        // вернет true, если было 
        // не может быть плюсов ниже умножения 
        public bool checkExpression(Expression exp)
        {
            bool f = false;
            Expression rightExp = exp.Right as Expression;
            Expression leftExp = exp.Left as Expression;
            if (rightExp != null)
            {
                // если встретили умножение или деление
                if (exp.state == State.OPER_DIV || exp.state == State.OPER_MPY)
                {
                    if (rightExp.state == State.OPER_MINUS || rightExp.state == State.OPER_PLUS)
                    {
                        // подвяжем выражение с умножением вниз слева
                        Expression tempExp = new Expression(exp.Left, exp.Oper, rightExp.Left);
                        tempExp.state = exp.state;
                        exp.Left = tempExp;
                        // поднимем операцию справа наверх
                        exp.Oper = rightExp.Oper;
                        exp.state = rightExp.state;
                        exp.Right = rightExp.Right;
                        f = true;
                    }
                }
                f = f | checkExpression(rightExp);
            }
            if (leftExp != null)
            {
                if (leftExp.state == State.OPER_MINUS || leftExp.state == State.OPER_PLUS)
                {
                    // подвяжем выражение с умножением вниз c
                    Expression tempExp = new Expression(leftExp.Right, exp.Oper, exp.Right);
                    tempExp.state = exp.state;
                    exp.Right = tempExp;
                    // поднимем операцию справа наверх
                    exp.Oper = leftExp.Oper;
                    exp.state = leftExp.state;
                    exp.Left = leftExp.Left;
                    f = true;
                }
                f = f | checkExpression(leftExp);
            }
            return f;
        }


        public bool PlusOrMinus(Expression exp) => exp.state == State.OPER_MINUS || exp.state == State.OPER_PLUS;

        public bool DivOrMpy(Expression exp) => exp.state == State.OPER_DIV || exp.state == State.OPER_MPY;
    }
}
