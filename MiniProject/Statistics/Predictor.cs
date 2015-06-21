using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProject.Data;

namespace MiniProject.Statistics
{
    abstract class Predictor<T, K, M, I> where I : IItem<T, K, M>
    {
        protected DataSet<T, K, M, I> TrainDataSet;

        public Predictor(DataSet<T, K, M, I> train)
        {
            this.TrainDataSet = train;
        }

        public double RMSE(DataSet<T, K, M, I> TestDataSet)
        {
            double sumRMP = 0.0;

            foreach (I testItem in TestDataSet.GetItems())
            {
                double prediction = PredictRating(testItem.GetUserID(), testItem.GetShearedItemID());
                double rating = TestDataSet.GetRating(testItem.GetUserID(), testItem.GetShearedItemID());
                sumRMP += Math.Pow(rating - prediction, 2);
            }

            double rmse = Math.Sqrt(sumRMP / TestDataSet.GetItems().Count);
            return rmse;
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

        public abstract double PredictRating(T activeUserID, M shearedItemID);
        public abstract void TrainModel();

        public abstract Dictionary<T, double> ClacUserWeights(T userID);
        public abstract Dictionary<T, double> GetKNN(T userID, int k);
    }
}
