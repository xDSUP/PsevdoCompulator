using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    class Lexeme
    {
        public enum LexemType
        {
            CONSTANT,
            CONSTANT_DOUBLE,
            ID, // идентификатор
            LIMITERS, // разделители
            KEY_WORD // ключевое слово языка
        }
        //значение лексемы
        public string Text { get; }
        public LexemType type { get; set; }

        public Lexeme(string text, LexemType type)
        {
            Text = text;
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is Lexeme lexeme &&
                   Text == lexeme.Text &&
                   type == lexeme.type;
        }


    }
}
