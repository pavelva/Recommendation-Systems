﻿using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class PearsonD<T, K, M, I> : OnlinePredictor<T, K, M, I> where I : IItem<T, K, M>
    {
        public PearsonD(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            double sumMultUA = 0.0, sumUMA = 0.0, sumAMA = 0.0;
            List<M> sharedItems = TrainDataSet.getSharedItems(activeUserID, userID);

            foreach (M itemID in sharedItems)
            {
                double UMA = TrainDataSet.GetRating(userID, itemID) - TrainDataSet.GetUserAverage(userID);
                double AMA = TrainDataSet.GetRating(activeUserID, itemID) - TrainDataSet.GetUserAverage(activeUserID);

                sumMultUA += (UMA * AMA);
                sumUMA += Math.Pow(UMA, 2);
                sumAMA += Math.Pow(AMA, 2);
            }

            double weight = sumMultUA / (Math.Sqrt(sumUMA) * Math.Sqrt(sumAMA));
            //ignore all the nagative weights.
            if (sumUMA == 0 || sumAMA == 0) return 0;
            //Console.WriteLine(weight);
            return (weight > 0) ? weight : 0;
            //return weight;
        }
    }
}
