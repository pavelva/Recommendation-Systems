using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProject.Data;


namespace MiniProject.Statistics
{
    abstract class OnlinePredictor<T, K, M, I> : Predictor<T, K, M, I> where I : IItem<T, K, M>
    {
        public OnlinePredictor(DataSet<T, K, M, I> dataSet) : base(dataSet)
        {
            
        }

        public override double PredictRating(T activeUserID, M shearedItemID)
        {
            double sumW = 0.0, sumWR = 0.0;

            //foreach (T userID in this.TrainDataSet.GetUserIDs())
            foreach (T userID in this.TrainDataSet.getUsersByItems()[shearedItemID].Keys)
            {
                if (userID.Equals(activeUserID)) 
                    continue;
                if (TrainDataSet.GetRating(userID, shearedItemID) == 0)
                    continue;
                double weight = GetWeight(activeUserID, userID);
                double rating = TrainDataSet.GetRating(userID, shearedItemID);
                sumW += weight;
                sumWR += weight * (rating - TrainDataSet.GetUserAverage(userID));
            }

            if (sumW == 0) return TrainDataSet.getItemAverage(shearedItemID);
            double score = TrainDataSet.GetUserAverage(activeUserID) + (sumWR / sumW);

            if (Double.IsNaN(score)) score = TrainDataSet.getItemAverage(shearedItemID);
            if (Double.IsNaN(score)) return TrainDataSet.getItemAverage();
            if (score > 1) return 1.0;
            return score;
        }

        private double GetWeight(T activeUserID, T userID)
        {
            double weight = CalculateWeight(activeUserID, userID);
            return weight;
        }

        public override void TrainModel()
        {
            throw new InvalidOperationException();
        }

        public override Dictionary<T, double> ClacUserWeights(T user1)
        {
            Dictionary<T, double> usersWeights = new Dictionary<T, double>();

            foreach (T user2 in TrainDataSet.GetUserIDs())
            {
                if (user1.Equals(user2)) 
                    continue;
                else
                    usersWeights.Add(user2, GetWeight(user1, user2));
            }

            return usersWeights;
        }

        public override Dictionary<T, double> GetKNN(T userID, int k)
        {
            //return ClacUserWeights(userID).OrderBy(w => w.Value).Take(k).Select(w => w.Key).ToList();
            Dictionary<T, double> allUserWeights = ClacUserWeights(userID);
            return allUserWeights.OrderBy(w => -w.Value).Take(k).ToDictionary(p => p.Key, p => p.Value);
        }

        protected abstract double CalculateWeight(T activeUserID, T userID);
    }
}
