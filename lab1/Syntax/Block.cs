namespace lab1
{
    internal class Block
    {
        public Block()
        {
        }

        public Block(Lexeme lexeme)
        {
            Lexeme = lexeme;
        }

        public Lexeme Lexeme { get; set; }
        public Block LeftChild { get; set; }
        public Block RightChild { get; set; }
    }
}