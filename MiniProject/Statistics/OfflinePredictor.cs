using Assignment1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1.Statistics
{
    abstract class OfflinePredictor<T, K, M, I> : Predictor<T, K, M, I> where I : IItem<T, K, M>
    {
        protected DataSet<T, K, M, I> ValidationDataSet;
        //protected Dictionary<T, Dictionary<T, double>> UserDistances;

        public OfflinePredictor(DataSet<T, K, M, I> train, DataSet<T, K, M, I> validation) : base(train)
        {
            this.ValidationDataSet = validation;
            //this.UserDistances = new Dictionary<T, Dictionary<T, double>>();
        }

        public double VMult(List<double> v1, List<double> v2)
        {
            double multSum = 0.0;

            for (int i = 0; i < v1.Count; i++)
            {
                multSum += v1[i] * v2[i];
            }

            return multSum;
        }

        public double VDistance(List<double> v1, List<double> v2)
        {
            double multSum = 0.0;

            for (int i = 0; i < v1.Count; i++)
            {
                multSum += Math.Pow(v1[i] - v2[i], 2);
            }

            return Math.Sqrt(multSum);
        }

        public override Dictionary<T, double> ClacUserWeights(T user1)
        {
            Dictionary<T, double> dis = new Dictionary<T, double>();

            foreach (T user2 in TrainDataSet.GetUserIDs())
            {
                if (user1.Equals(user2)) continue;
                dis.Add(user2, getUserDistances(user1, user2));
            }

            //return UserDistances[user1];
            return dis;
        }

        public override Dictionary<T, double> GetKNN(T userID, int k)
        {
            Dictionary<T, double> allUserWeights = ClacUserWeights(userID);
            return allUserWeights.OrderBy(w => w.Value).Take(k).ToDictionary(p => p.Key, p => 1 - p.Value);
        }

        public abstract double getUserDistances(T user1, T user2);
    }
}
