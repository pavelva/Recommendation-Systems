using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProject.Data;


namespace MiniProject.Statistics
{
    abstract class OnlinePredictor<T, K, M, I> : Predictor<T, K, M, I> where I : IItem<T, K, M>
    {
        Dictionary<T, Dictionary<T, double>> weights;
        Dictionary<T, double> sumWeights;

        public OnlinePredictor(DataSet<T, K, M, I> dataSet) : base(dataSet)
        {
            this.weights = new Dictionary<T, Dictionary<T, double>>();
            this.sumWeights = new Dictionary<T, double>();

            foreach (T user in this.TrainDataSet.GetUserIDs())
            {
                foreach (T user2 in this.TrainDataSet.GetUserIDs())
                {
                    if (user.Equals(user2)) continue;

                    GetWeight(user, user2);
                }

                sumWeights.Add(user, weights[user].Sum(u => u.Value));
            }
        }

        public override double PredictRating(T activeUserID, M shearedItemID)
        {
            double sumW = 0.0, sumWR = 0.0;
            //List<T> sharedItems = this.TrainDataSet.getUsersByItems()[shearedItemID].Keys.ToList();

            //foreach (T userID in this.TrainDataSet.GetUserIDs())
            if (!this.TrainDataSet.getUsersByItems().ContainsKey(shearedItemID))
                return 0;

            foreach (T userID in this.TrainDataSet.getUsersByItems()[shearedItemID].Keys)
            {
                if (userID.Equals(activeUserID)) 
                    continue;

                //double weight = GetWeight(activeUserID, userID);
                double weight = weights[activeUserID][userID];
                double rating = TrainDataSet.GetRating(userID, shearedItemID);

                //sumW += weight;
                //sumWR += weight * (rating - TrainDataSet.GetUserAverage(userID));
                sumWR += weight * rating;
            }

            sumW = sumWeights[activeUserID];
            if (sumW == 0) return TrainDataSet.getItemAverage(shearedItemID);
            //double score = TrainDataSet.GetUserAverage(activeUserID) + (sumWR / sumW);
            double score = sumWR / sumW;

            if (Double.IsNaN(score)) return 0.5;
            if (score > 1) return 1.0;
            return score;
        }

        
        private double GetWeight(T activeUserID, T userID)
        {
            if (weights.ContainsKey(activeUserID))
            {
                if(weights[activeUserID].ContainsKey(userID))
                    return weights[activeUserID][userID];
            }

            //if (weights.ContainsKey(userID))
            //{
            //    if (weights[userID].ContainsKey(activeUserID))
            //        return weights[userID][activeUserID];
            //}

            if (!weights.ContainsKey(activeUserID))
                weights.Add(activeUserID, new Dictionary<T, double>());
            if (!weights.ContainsKey(userID))
                weights.Add(userID, new Dictionary<T, double>());

            double weight = CalculateWeight(activeUserID, userID);
            weights[activeUserID].Add(userID, weight);
            weights[userID].Add(activeUserID, weight);
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
