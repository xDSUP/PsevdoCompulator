using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace lab1
{
    class LexicalAnalizator
    {
        /// <summary>
        /// . Входной язык содержит операторы условия if … then … else и if … then,
        /// разделённые символом ; (точка с запятой). Операторы условия содержат 
        /// идентификаторы, знаки сравнения <, >, =, десятичные числа с плавающей 
        /// точкой, знак присваивания (:=).
        /// </summary>
        String allTextProgram; // анализируемый текст
        public List<Lexeme> tableID { get; } // таблица идентификаторов(никаких повторов)
        public IdHashTable<Lexeme> idHashTable;
        public List<Lexeme> tableLexemes { get; } // таблица лексем(могут быть повторы)
        private string temp; // для грамо

        static String operators = "=-+/*^<>";
        static String limiters = "():;={}<>+- \n/*^";
        static String[] reservedWords = { "if", "then", "else" };


        // при создании удаляет все пробелы из текста
        public LexicalAnalizator(string text)
        {
            // убираем все двойные пробелы
            while(text.Contains("  "))
                text = text.Replace("  ", " ");

            this.allTextProgram = text;
            tableID = new List<Lexeme>();
            tableLexemes = new List<Lexeme>();
            idHashTable = new IdHashTable<Lexeme>();
        }


        // анализ всего текста, если успешно проанализированно, вернет true
        public bool analysis()
        {
            string temp = "";
            Regex regexEng = new Regex(@"[A-Za-z_]");
            Regex regexNum = new Regex(@"[0-9,]");
            Lexeme.LexemType tempType = Lexeme.LexemType.ID;
            for (int i = 0; i < allTextProgram.Length; i++)
            {
                Char ch = allTextProgram[i];
                // проверка на английский символ либо пробел
                if(regexEng.Match(ch.ToString()).Success)
                {
                    //
                    if (String.IsNullOrEmpty(temp))
                        tempType = Lexeme.LexemType.ID;
                    temp += ch;
                    tempType = Lexeme.LexemType.ID;
                    continue; // продолжаем
                }
                // проверка на цифры и запятые
                if (regexNum.Match(ch.ToString()).Success)
                {
                    // если это запятая
                    if(ch == ',')
                    {
                        // если до этого тоже стоят цифры
                        if(int.TryParse(temp, out int int_temp))
                        {
                            tempType = Lexeme.LexemType.CONSTANT_DOUBLE;
                        }
                        else // если прежде были не цифры, значит это идентификатор и пора его сохранить
                        {
                            result(new Lexeme(temp, tempType));
                            temp = "";
                            continue;
                        }
                    }
                    if (String.IsNullOrEmpty(temp))
                        tempType = Lexeme.LexemType.CONSTANT;
                    temp += ch;
                    continue;
                }
                // если это разделители
                if (limiters.Contains(ch.ToString()))
                {
                    // если что-то было, сохраним
                    if (!String.IsNullOrEmpty(temp))
                        result(new Lexeme(temp, tempType));
                    // обнулим темп
                    temp = ch.ToString();
                    tempType = Lexeme.LexemType.LIMITERS;
                    if (ch==' ' || ch=='\n')
                    {
                        temp = "";
                        continue;
                    }
                    // если это знак присваивания ":="
                    if (ch == ':')
                    {
                        if (allTextProgram.Count() >= i+2 && allTextProgram[i + 1] == '=')
                        {
                            temp += allTextProgram[i + 1].ToString();
                            //result(new Lexeme(temp, Lexeme.LexemType.LIMITERS));
                            // переставим указатель дальше за равно
                            tempType = Lexeme.LexemType.OPERATOR;
                            i+=1;
                        }
                        else
                        {
                            //TODO: сообщение об ошибке
                        }
                    }

                    // если это знак операции
                    if (operators.Contains(ch.ToString()))
                    {
                        tempType = Lexeme.LexemType.OPERATOR;
                    }
                    // TODO: обрабатывать области видимости
                    // открытие новой области видимости
                    //if( ch == '{')
                    //{
                    //    idHashTable.initializeScope();
                    //}
                    //if (ch == '}')
                    //{
                    //    idHashTable.finalizeScope();
                    //}
                    result(new Lexeme(temp, tempType));
                    temp = "";
                }
            }
            if (!String.IsNullOrEmpty(temp))
                result(new Lexeme(temp, tempType));
            return true;
        }

        // финально обрабатывает готовую выделенную из текста лексему
        private void result(Lexeme lexeme)
        {
            // проверка на ключевые слова
            if (reservedWords.Contains(lexeme.Text))
            {
                lexeme.type = Lexeme.LexemType.KEY_WORD;
            }
            // если это константа или ид возможно нужно добавить в табилцу
            if (lexeme.type == Lexeme.LexemType.ID ||
                   lexeme.type == Lexeme.LexemType.CONSTANT ||
                   lexeme.type == Lexeme.LexemType.CONSTANT_DOUBLE)
            {
                // если в таблице идентификаторов еще нет этой лексемы
                if (!tableID.Contains(lexeme))
                {
                    tableID.Add(lexeme);
                }
                if (lexeme.type == Lexeme.LexemType.ID)
                { 
                    // TODO: обработка вложенности
                    // если еще не был добавлен в таблицу, тогда добавим
                    if(idHashTable.lookUp(lexeme) == null)
                    {
                        idHashTable.insert(lexeme);
                    }
                }
            }
            tableLexemes.Add(lexeme);
        }

        public override string ToString()
        {
            String res = "";
            foreach(Lexeme l in tableLexemes)
            {
                if (tableID.Contains(l))
                {
                    res += "<id" + tableID.IndexOf(l) + ">";
                    continue;
                }
                res += l.Text;
            }
            return res;
        }
    }
}
