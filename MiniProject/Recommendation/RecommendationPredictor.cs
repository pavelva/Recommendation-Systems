using MiniProject.Data;
using MiniProject.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2.Recommendation
{
    class RecommendationPredictor<T, K, M, I> : RecommendationBase<T, K, M, I> where I : IItem<T, K, M>
    {
        public RecommendationPredictor(DataSet<T, K, M, I> train, Predictor<T, K, M, I> predictor) : base(train, predictor) { }

        public override List<M> Recommendation(T userID, int n)
        {
            Dictionary<M, double> itemPredictions = new Dictionary<M, double>(n);
            Dictionary<M, Dictionary<T, I>> usersByItems = TrainDataSet.getUsersByItems();

            List<M> sharedIds = usersByItems.Keys.Where(m => !TrainDataSet.UserContainsItem(userID, m)).ToList();

            
            foreach(M sharedID in sharedIds)
            {
                /*double prediction = Predictor.PredictRating(userID, sharedID);
                if (itemPredictions.Count < n)
                    itemPredictions.Add(sharedID, prediction);
                else
                {
                    double minPred = itemPredictions.Values.Min();
                    if (prediction > minPred)
                    {
                        M minUser = itemPredictions.Keys.Where(u => itemPredictions[u] == minPred).First();
                        itemPredictions.Remove(minUser);
                        itemPredictions.Add(sharedID, prediction);
                    }

                    if (minPred >=4.0)
                        break;
                }*/
                double prediction = Predictor.PredictRating(userID, sharedID);
                itemPredictions.Add(sharedID, prediction);
            }

            List<M> recommendation = itemPredictions.Keys.OrderBy(m => -itemPredictions[m]).Take(n).ToList();
            return recommendation;
        }
    }
}
