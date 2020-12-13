using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.Hashtable
{
    public class HashTableNode<T> where T : IHastable
    {
        private static int _newId = 0;
        public double Value { get; set; }
        public T Lexeme { get; set; }
        public int Level { get; set; }
        public HashTableNode<T> id { get; set; }
        public bool isDead { get; set; }

        public HashTableNode (T lexeme, int level)
        {
            Lexeme = lexeme;
            Level = level;
            isDead = false;
        }

        public override string ToString()
        {
            return Lexeme.ToString() + " " + Value;
        }
    }
}
