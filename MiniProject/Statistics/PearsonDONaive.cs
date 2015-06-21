using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class PearsonDONaive<T, K, M, I> : PearsonDNaive<T, K, M, I> where I : IItem<T, K, M>
    {
        public PearsonDONaive(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            return CalculateWeightDemo(activeUserID, userID);
        }
    }
}
