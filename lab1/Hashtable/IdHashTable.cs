using lab1.Hashtable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{ 
    public class IdHashTable<T> where T : IHastable
    {
        /// <summary>
        ///  стек, где будем хранить все элементы таблицы
        /// </summary>
        private List<HashTableNode<T>> _stack;
        /// <summary>
        /// Хранит указатели на элементы в стеке по адресу хеша элемента
        /// </summary>
        private HashTableNode<T>[] _array; 
        
        // размер таблицы
        private int _size;
        /// <summary>
        /// текущий уровень вложенности
        /// </summary>
        private int _currentLevel;

        public List<HashTableNode<T>> Elems { get => _stack; }
        public int CurrentLevel { get => _currentLevel; }
        
        // константа для вычисления хэша
        private readonly double C = new Random().NextDouble();
        // стартовый размер таблицы
        private readonly int START_SIZE = 2;

        public IdHashTable()
        {
            _size = START_SIZE;
            _stack = new List<HashTableNode<T>>();
            _array = new HashTableNode<T>[_size];
            _currentLevel = 0;
        }

        /// <summary>
        /// ищет в таблице запись
        /// </summary>
        /// <param name="lexeme">искомая запись</param>
        /// <returns>возвращает запись, хранящуюся в таблице в ячейке с адресом = хешу, 
        /// если таковая существует. 
        /// Иначе она возвращает значение null , указывающее на то, что такой записи нет;</returns>
        public HashTableNode<T> lookUp(T lexeme)
        {
            var hash = _getHashCode(lexeme);
            var item = _array[hash];

            if (item == null) // по этому адресу ничего нет
            {
                return null;
            }
            // есть запись по этому хешу и это нужный элемент
            if (item.Lexeme.Equals(lexeme))
            {
                return item;
            }
            else // нужно найти новую позицию после рехеширования
            {
                int i = 1;
                int newHash = (i + hash) % _size;
                while (newHash != hash)
                {
                    var curItem = _array[newHash];
                    // если там пусто, значит рехеша не было, и такого элем-та просто нет
                    if (curItem == null)
                    {
                        return null;
                    }
                    else // если не пусто, нужно проверить, id того элемента
                    {
                        if (curItem.Lexeme.Equals(lexeme))
                        {
                            return curItem;
                        }
                    }
                    newHash = (i++ + hash) % _size;
                }
            }
            // ошибочка
            return null;
        }


        private void _insertInTable(HashTableNode<T> currentId)
        {
            var hash = _getHashCode(currentId.Lexeme);
            // если не было такого хеша, вставим
            if (_array[hash] == null)
            {
                _array[hash] = currentId;
            }
            else // коллизия
            {
                // одинаковое имя переменной 
                if (_array[hash].Lexeme.Equals(currentId.Lexeme))
                {
                    // связываем новую и старую
                    currentId.id = _array[hash];
                    // установим ссылку на новую запись
                    _array[hash] = currentId;
                }
                else
                {
                    // ищем свободный слот, проверяя каждый следующий слот в таблице, 
                    // пока не дойдем до текущего хеша или не найдем пустой
                    int i = 1;
                    while (true)
                    {
                        var newHash = (i++ + hash) % _size;
                        if (_array[newHash] == null)
                        {
                            _array[newHash] = currentId;
                            break;
                        }
                        // проверка на текущий хеш
                        if (newHash == hash) // значит мы обошли таблицу
                        {
                            // TODO: завернуть в try
                            resize();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// увеличивает размер таблицы
        /// </summary>
        private void resize()
        {
            _array = new HashTableNode<T>[_size = getNewSize()];
            //добавим в новую таблицу, все что в стеке
            foreach (var id in _stack)
            {
                _insertInTable(id);
            }
        }

        /// <summary>
        /// вставлет идентификатор name в ячейке с номером хеша.
        /// </summary>
        /// <param name="elem"></param>
        public void insert(T elem)
        {            
            var currentElem = new HashTableNode<T>(elem, _currentLevel);
            // закинем на вершину стека
            _stack.Insert(0,currentElem);
            _insertInTable(currentElem);
        }

        /// <summary>
        /// Увеличивает текущий уровень вложенности
        /// </summary>
        public void initializeScope()
        {
            _currentLevel++;
        }

        /// <summary>
        /// С вершины стека берет все переменнные с текущим уровнем вложенности
        /// И обновляет соотв-щим образом указатель в массиве
        /// Если компилятору необходимо сохранить таблицы 
        /// всех уровней для дальнейшего использования, эта функция может 
        /// оставить таблицу в памяти или записать ее в файл на диск и 
        /// освободить занимаемую ею память.
        /// </summary>
        public void finalizeScope()
        {
            // получаем все ид текущего уровня
            foreach (var elem in _stack.Where(t=>t.Level == _currentLevel))
            {
                // проверяет есть ли переменные с таким же именем на других уровнях
                if (elem.id == null) // если нет
                {
                    elem.isDead = true;
                }
                else
                {
                    // заменяем ссылку в массиве на обьявление переменной на 
                    // другом уровне
                    var item = _array[_getHashCode(elem.Lexeme)];
                    if (item == elem) // если не было рехеширования
                        item = elem.id;
                    else
                    {
                        //TODO: обработать рехеширование
                    }
                }
                // удаляем из стека
                _stack.Remove(elem);
            }
        }

        private int _getHashCode(T key)
        {
            return (int)Math.Floor(_size * (C * Math.Abs(key.GetHashCode()) % 1));
        }

        private int getNewSize() => _size * 2;
    }
}
