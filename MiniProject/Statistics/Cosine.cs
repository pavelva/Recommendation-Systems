using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProject.Data;

namespace MiniProject.Statistics
{
    class Cosine<T, K, M, I> : OnlinePredictor<T, K, M, I> where I : IItem<T, K, M>
    {
        public Cosine(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            double sumMultUMA = 0.0, sumRUI = 0.0, sumRAI = 0.0;
            List<M> joinedItems = TrainDataSet.getJoinedItems(activeUserID, userID);

            foreach (M itemID in joinedItems)
            {
                double UMA = TrainDataSet.GetRating(userID, itemID) * TrainDataSet.GetRating(activeUserID, itemID);
                double RUI = Math.Pow(TrainDataSet.GetRating(userID, itemID), 2);
                double RAI = Math.Pow(TrainDataSet.GetRating(activeUserID, itemID), 2);
                //Console.WriteLine("-- {0} {1} {2}", UMA, RUI, RAI);
                sumMultUMA += UMA;
                sumRUI += RUI;
                sumRAI += RAI;
            }

            double weight = sumMultUMA / (Math.Sqrt(sumRUI) * Math.Sqrt(sumRAI));
            //Console.WriteLine(weight);
            return weight;
        }
    }
}
