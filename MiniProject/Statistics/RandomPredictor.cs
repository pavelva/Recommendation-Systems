using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniProject.Statistics
{
    class RandomPredictor<T, K, M, I> : Predictor<T, K, M, I> where I : IItem<T, K, M>
    {
        private Dictionary<T, List<double>> UserRatingCounetr;

        public RandomPredictor(DataSet<T, K, M, I> train) : base(train)
        {
            UserRatingCounetr = new Dictionary<T, List<double>>();
        }

        public override double PredictRating(T activeUserID, M shearedItemID)
        {
            if (!UserRatingCounetr.ContainsKey(activeUserID))
            {
                GenRatingCounetr(activeUserID);
            }
            return SampleRandomPrediction(activeUserID);
        }

        private double SampleRandomPrediction(T activeUserID)
        {
            Random rand = new Random();
            int size = UserRatingCounetr[activeUserID].Count;

            return UserRatingCounetr[activeUserID][rand.Next(0, size)];
        }

        private void GenRatingCounetr(T activeUserID)
        {
            List<double> ratingCounter = new List<double>();

            foreach (I item in TrainDataSet.GetItems(activeUserID))
            {
                ratingCounter.Add(item.GetRating());
            }

            UserRatingCounetr.Add(activeUserID, ratingCounter);
        }

        public override void TrainModel()
        {
            throw new NotSupportedException();
        }

        public override Dictionary<T, double> ClacUserWeights(T userID)
        {
            throw new NotSupportedException();
        }

        public override Dictionary<T, double> GetKNN(T userID, int k)
        {
            throw new NotSupportedException();
        }
    }
}
