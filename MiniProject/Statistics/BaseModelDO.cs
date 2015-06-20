using System;
using System.Collections.Generic;
using MiniProject.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class BaseModelDO<T, K, M, I> : BaseModelD<T, K, M, I>where I : IItem<T, K, M>
    {
        public BaseModelDO(DataSet<T, K, M, I> train, DataSet<T, K, M, I> validation)
            : base(train, validation, 3){}
    }
}
