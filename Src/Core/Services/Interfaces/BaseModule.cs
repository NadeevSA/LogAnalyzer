using Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    public abstract class BaseModule
    {
        public virtual int IsLogger(string sourceString, string nameLogger, HierarchyResult hierarchyResult)
        {
            var result = sourceString.IndexOf(nameLogger);
            if (result != -1)
            {
                hierarchyResult.ResultText = "Лог есть";
            }
            else
            {
                hierarchyResult.ResultText = "Лога нет";
            }

            return result != -1 ? 1 : 0;
        }
    }
}
