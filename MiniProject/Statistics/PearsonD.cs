using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class PearsonD<T, K, M, I> : Pearson<T, K, M, I> where I : IItem<T, K, M>
    {
        public PearsonD(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            double itemsWeigth = base.CalculateWeight(activeUserID, userID);
            double demoWeigths = CalculateWeigthDemo(activeUserID, userID);
            return 0;
        }

        private double CalculateWeigthDemo(T activeUserID, T userID)
        {
            throw new NotImplementedException();
        }
       
    }
}
