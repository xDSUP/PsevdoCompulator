using lab1.Hashtable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public class Lexeme : IHastable
    {
        public enum LexemType
        {
            CONSTANT,
            CONSTANT_DOUBLE,
            ID, // идентификатор
            LIMITERS, // разделители
            KEY_WORD, // ключевое слово языка
            OPERATOR
        }
        //значение лексемы
        public string Text { get; }
        public LexemType type { get; set; }


        public Lexeme(string text, LexemType type)
        {
            Text = text;
            this.type = type;
        }

        public override string ToString()
        {
            return Text;
        }

        public override bool Equals(object obj)
        {
            return obj is Lexeme lexeme &&
                   Text == lexeme.Text &&
                   type == lexeme.type;
        }

        /// <summary>
        /// Проверит является ли лексема константой либо идентификатором
        /// </summary>
        /// <returns></returns>
        public bool isConst()
        {
            return type == LexemType.CONSTANT ||
                type == LexemType.CONSTANT_DOUBLE ||
                type == LexemType.ID;
        }

        public override int GetHashCode()
        {
            int hashCode = -1683395749;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            return hashCode;
        }
    }
}
