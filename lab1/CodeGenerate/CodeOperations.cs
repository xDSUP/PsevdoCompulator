using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.CodeGenerate
{
    public enum CodeOperationType
    {
        /// <summary>
        /// с(m) → сумматор
        /// </summary>
        LOAD,
        /// <summary>
        /// с(сумматор) + с(m) → сумматор
        /// </summary>
        ADD,
        /// <summary>
        /// с(сумматор) * с(m) → сумматор
        /// </summary>
        MPY,
        /// <summary>
        /// с(сумматор) / с(m) → сумматор
        /// </summary>
        DIV,
        /// <summary>
        /// с(сумматор) - с(m) → сумматор
        /// </summary>
        SUB,
        /// <summary>
        /// с(сумматор) → m // помещает результат в перем
        /// </summary>
        STORE,

        /// <summary>
        /// с(сумматор) = m → сумматор
        /// </summary>
        EQUAL,
        /// <summary>
        /// с(сумматор) > m → сумматор
        /// </summary>
        GT,
        /// <summary>
        /// с(сумматор) < m → сумматор
        /// </summary>
        LT,

        //нужны ли аналоги для переменной?

        /// <summary>
        /// прыжок к метке
        /// </summary>
        JMP,
        /// <summary>
        /// прыжок к метке, если в сумматоре 1
        /// </summary>
        JE,
        /// <summary>
        /// прыжок к метке, если в сумматоре 0
        /// </summary>
        JZ
    }

    public class CodeOperation
    {
        /// <summary>
        ///  тип операции
        /// </summary>
        public CodeOperationType Type { get; set; }
        /// <summary>
        /// параметр
        /// </summary>
        public object Parametr { get; set; }

        public CodeOperation(CodeOperationType type, object parametr)
        {
            this.Type = type;
            this.Parametr = parametr;
        }

        public override string ToString()
        {
            return $"{Type.ToString()} {Parametr?.ToString()}";
        }
    }
}
