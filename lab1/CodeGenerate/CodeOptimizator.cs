using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace lab1.CodeGenerate
{
    class CodeOptimizator
    {
        List<CodeBlock> codeBlocks;
        IdHashTable<Lexeme> hashTable;

        public CodeOptimizator(List<CodeBlock> operations, IdHashTable<Lexeme> hashTable)
        {
            this.codeBlocks = operations;
            this.hashTable = hashTable;
        }

        public void Optimize()
        {
            // проходимся по каждому блоку
            foreach (var block in codeBlocks)
            {
                bool isChanged = true, res=true;
                while (isChanged | res)
                {
                    res = false;
                    while (isChanged) // пока что-то меняется
                    {
                        isChanged = ruleOne(block);
                        res |= isChanged;
                    }
                        
                    isChanged = true;
                    while (isChanged)
                    {
                        isChanged = ruleTwo(block);
                        
                    }
                        
                    isChanged = true;
                    while (isChanged)
                    {
                        isChanged = ruleThree(block);
                        res |= isChanged;
                    }
                }
                
            }
        }

        // 1 правило: если идет последовательность 
        // load var0
        // store var1
        // если за ней следует другой оператор LOAD и нет перехода к оператору STORE var1,
        // а последующие вхождения var1 будут заменены на α вплоть до того места, 
        // где появляется другой оператор STORE var1 (но исключая его)
        private static bool ruleOne(CodeBlock block)
        {
            // индексы операций, которые можно сократить
            List<int> deletedIndexs = new List<int>();
            int i = 0;
            foreach (var oper in block.operations)
            {
                // всретили лоад и он не последний
                if (i < block.operations.Count - 2 && oper.Type == CodeOperationType.LOAD)
                {
                    // сразу за ним есть стор
                    var nextOper = block.operations[i + 1];
                    if (nextOper.Type == CodeOperationType.STORE
                        && oper.Parametr != nextOper.Parametr
                        && nextOper.Parametr.StartsWith("VAR")) // и это временная переменная
                    {
                        // дальше всегда лоад либо иф
                        if (block.operations[i+2].Type == CodeOperationType.LOAD)
                        {
                            var temp = block.operations.GetRange(i+2, block.operations.Count-i-1-2);
                            // ближайщая операция
                            var indNextStore = temp.
                                FindIndex((e) => e.Type == CodeOperationType.STORE && e.Parametr == nextOper.Parametr);
                            if(indNextStore < 0) // значит дальше нигде не используется
                            {
                                foreach (var t in block.operations.Skip(i + 2).Where((e) => e.Parametr == nextOper.Parametr))
                                {
                                    t.Parametr = oper.Parametr;
                                } 
                                    
                            }
                            else
                            {
                                // нужно заменить именно до того первого store
                                foreach (var t in block.operations.GetRange(i+2, indNextStore).Where((e) => e.Parametr == nextOper.Parametr))
                                {
                                    t.Parametr = oper.Parametr;
                                }
                            }

                            deletedIndexs.Add(i);
                            deletedIndexs.Add(i + 1);
                            break;
                            i++;
                        }
                    }
                }
                i++;
            }

            if(deletedIndexs.Count == 0)
            {
                return false;
            }
            {
                deletedIndexs.Reverse();
                foreach (var ind in deletedIndexs)
                {
                    block.operations.RemoveAt(ind);
                }
                return true;
            }
            
        }

        // 2 правило: если идет последовательность 
        // store var0
        // load var0
        // можно удалить из программы при условии, что это временная переменная,
        // если не временная следует проверить не сохраняется ли она еще куда-то потом
        // на случай х:=1; у:=х;
        private static bool ruleTwo(CodeBlock block)
        {
            List<int> deletedIndexs = new List<int>();
            int i = 0;
            foreach (var oper in block.operations)
            {
                // всретили лоад и он не последний
                if (i < block.operations.Count - 1 && oper.Type == CodeOperationType.STORE)
                {
                    // сразу за ним есть стор с таким же именем
                    var nextOper = block.operations[i + 1];
                    if (nextOper.Type == CodeOperationType.LOAD
                        && oper.Parametr == nextOper.Parametr
                        && oper.Parametr.StartsWith("VAR")) // только для временных переменных такая штука будет работать(на всякий)
                    {
                        //if(block.operations[i + 2].Type == CodeOperationType.)
                        // удаляем все наф
                        deletedIndexs.Add(i);
                        deletedIndexs.Add(i + 1);
                        break;

                    }
                }
                i++;
            }

            if (deletedIndexs.Count == 0)
            {
                return false;
            }
            {
                deletedIndexs.Reverse();
                foreach (var ind in deletedIndexs)
                {
                    block.operations.RemoveAt(ind);
                }
                return true;
            }
        }

        // 3 правило: если идет последовательность 
        // load var0
        // load var1
        // можно удалить из программы, это издержки генерации 
        private static bool ruleThree(CodeBlock block)
        {
            List<int> deletedIndexs = new List<int>();
            int i = 0;
            foreach (var oper in block.operations)
            {
                // всретили лоад и он не последний
                if (i < block.operations.Count - 1 && oper.Type == CodeOperationType.LOAD)
                {
                    // сразу за ним есть лоад с таким же именем
                    var nextOper = block.operations[i + 1];
                    if (nextOper.Type == CodeOperationType.LOAD
                        && oper.Parametr != nextOper.Parametr)
                        //&& oper.Parametr.StartsWith("VAR")) // только для временных переменных такая штука будет работать(на всякий)
                    {
                        //if(block.operations[i + 2].Type == CodeOperationType.)
                        // удаляем все наф
                        deletedIndexs.Add(i);
                        break;
                    }
                }
                i++;
            }

            if (deletedIndexs.Count == 0)
            {
                return false;
            }
            {
                deletedIndexs.Reverse();
                foreach (var ind in deletedIndexs)
                {
                    block.operations.RemoveAt(ind);
                }
                return true;
            }
        }
    }
}
