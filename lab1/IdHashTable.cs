using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    class Id
    {
        public String Identifier { get; set; }
        public int Level { get; set; }
        public Id id { get; set; }
        public bool isDead { get; set; }
        
        public Id(string identifier, int level)
        {
            Identifier = identifier;
            Level = level;
            isDead = false;
        }
    }

    class IdHashTable
    {
        // стек, где будем хранить все элементы таблицы
        private List<Id> _stack;
        private Id[] _array; 
        
        // размер таблицы
        private int _size;
        private int _currentLevel;
        
        // константа для вычисления хэша
        private readonly double C = new Random().NextDouble();

        public IdHashTable()
        {
            _stack = new List<Id>();
            _array = new Id[10];
            _currentLevel = 0;
        }

        /// <summary>
        /// возвращает запись, хранящуюся в таблице в ячейке h(name), 
        /// если таковая существует. 
        /// Иначе она возвращает значение null , указывающее на то, что такой записи нет;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Id lookUp(String id)
        {
            var hash = _getHashCode(id);
            var item = _array[hash];

            if (item == null) // такого хеша у нас нет
            {
                return null;
            }
            // есть такой хеш и это нужный элемент
            if (item.Identifier == id)
            {
                return item;
            }
            else // нужно найти новую позицию
            {
                int i = 1;
                int newHash = (i + hash) % _size;
                while (newHash != hash)
                {
                    if (_array[newHash] == null)
                    {
                        return _array[newHash];

                    }
                    newHash = (i++ + hash) % _size;
                }
            }
            // ошибочка
            return null;
        }


        private void _insertInTable(Id currentId)
        {
            var hash = _getHashCode(currentId.Identifier);
            // если не было такого хеша, вставим
            if (_array[hash] == null)
            {
                _array[hash] = currentId;
            }
            else // коллизия
            {
                // одинаковое имя переменной 
                if (_array[hash].Identifier == currentId.Identifier)
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
                        var newHash = (i + hash) % _size;
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

        private void resize()
        {
            _array = new Id[_size = getNewSize()];
            //добавим в новую таблицу, все что в стеке
            foreach (var id in _stack)
            {
                _insertInTable(id);
            }
        }

        /// <summary>
        /// сохраняющая идентификатор name в ячейке с номером h(name).
        /// </summary>
        /// <param name="id"></param>
        public void insert(String id)
        {            
            var currentId = new Id(id, _currentLevel);
            // закинем на вершину стека
            _stack.Insert(0,currentId);
            _insertInTable(currentId);
        }

        /// <summary>
        /// Эта функция связывает новую таблицу с таблицей предыдущего уровня
        /// и обновляет указатель текущего уровня, используемый функциями 
        /// LookUp и Insert;
        /// </summary>
        public void initializeScope()
        {
            _currentLevel++;
        }

        /// <summary>
        /// перенаправляет указатель текущего уровня на таблицу предыдущей 
        /// области видимости. Если компилятору необходимо сохранить таблицы 
        /// всех уровней для дальнейшего использования, эта функция может 
        /// оставить таблицу в памяти или записать ее в файл на диск и 
        /// освободить занимаемую ею память.
        /// </summary>
        public void finalizeScope()
        {
            // получаем все ид текущего уровня
            foreach (var id in _stack.Where(t=>t.Level == _currentLevel))
            {
                // проверяет есть ли переменные с таким же именем на других уровнях
                if (id.id != null) // если нет
                {
                    id.isDead = true;
                }
                else
                {
                    // заменяем ссылку в массиве на обьявление переменной на 
                    // другом уровне
                    _array[_getHashCode(id.Identifier)] = id.id;
                }
            }            
        }

        private int _getHashCode(String key)
        {
            return (int)Math.Floor(_size * ((C * key.GetHashCode()) % 1));
        }

        private int getNewSize() => _size * 2;
    }
}
