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


        // все они ставят 1 в сумматор, если тру
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

        //прыжок к метке, если такая есть иначе пропуск
        /// <summary>
        /// прыжок к метке, если такая есть иначе пропуск
        /// </summary>
        JMP,
        /// <summary>
        /// прыжок к метке, если в сумматоре 1
        /// </summary>
        JE,
        /// <summary>
        /// прыжок к метке, если в сумматоре 0
        /// </summary>
        JZ,
        /// <summary>
        /// Метка :<имя метки>
        /// </summary>
        POINT
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
        public string Parametr { get; set; }

        public bool deleted { get; set; }

        public CodeOperation(CodeOperationType type, object parametr)
        {
            this.Type = type;
            this.Parametr = parametr.ToString();
        }

        public override string ToString()
        {
            return $"{Type.ToString()} {Parametr?.ToString()}";
        }
    }
}
