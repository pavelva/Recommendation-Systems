using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class PearsonDO<T, K, M, I> : PearsonD<T, K, M, I> where I : IItem<T, K, M>
    {
        public PearsonDO(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            return CalculateWeightDemo(activeUserID, userID);
        }
    }
}
