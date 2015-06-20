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
            double demoWeigths = CalculateWeightDemo(activeUserID, userID);
            return (0.5 * itemsWeigth + 0.5 * demoWeigths);
        }

        protected double CalculateWeightDemo(T activeUserID, T userID)
        {
            double weight = 1;
            UserData active = TrainDataSet.GetUserData(activeUserID.ToString());
            UserData user = TrainDataSet.GetUserData(userID.ToString());
            if(!active.GetCountry().Equals(user.GetCountry()))
                weight -= 0.33;

            if(Math.Abs(active.GetAge() - user.GetAge()) > 5)
                weight -= 0.33;

            if (!active.GetGender().Equals(user.GetGender()))
                weight -= 0.33;

            return (weight == 0.01 ? 0 : weight);
        }
       
    }
}
