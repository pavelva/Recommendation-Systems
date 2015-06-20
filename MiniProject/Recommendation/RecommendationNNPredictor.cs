using MiniProject.Data;
using MiniProject.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2.Recommendation
{
    class RecommendationNNPredictor<T, K, M, I> : RecommendationBase<T, K, M, I> where I : IItem<T, K, M>
    {
        private static readonly int k = 20;

        public RecommendationNNPredictor(DataSet<T, K, M, I> train, Predictor<T, K, M, I> predictor) : base(train, predictor) { }

        public override List<M> Recommendation(T userID, int n)
        {
            Dictionary<T, double> KNN = Predictor.GetKNN(userID, k);

            return GetTopN(userID, KNN, n);
        }

        private List<M> GetTopN(T activeUserID, Dictionary<T, double> KNN, int n)
        {
            Dictionary<M, double> itemsRating = new Dictionary<M, double>();

            List<M> items = TrainDataSet.GetItems().Where(i => KNN.ContainsKey(i.GetUserID()) &&
                    !TrainDataSet.getUsersByItems()[i.GetShearedItemID()].ContainsKey(activeUserID)).
                    Select(i => i.GetShearedItemID()).GroupBy(x => x).Select(x => x.Key).ToList();

            foreach (M itemID in items)
            {
                int ratingUserNum = 0;
                itemsRating.Add(itemID, 0.0);
                foreach (T userID in KNN.Keys)
                {
                    if (TrainDataSet.GetRating(userID, itemID) != 0)
                    {
                        ratingUserNum++;
                        itemsRating[itemID] += KNN[userID];
                    }
                }
                itemsRating[itemID] = ratingUserNum == 0 ? 0 : itemsRating[itemID] / ratingUserNum;
            }
            
            List<M> recommendation = itemsRating.OrderBy(i => -i.Value).Take(n).Select(i => i.Key).ToList();
            return recommendation;
        }

        private Dictionary<T, double> Jaccard(T userID, List<T> KNN)
        {
            Dictionary<T, double> usersJWeights = new Dictionary<T,double>();

            foreach (T Nuser in KNN)
            {
                double sheared = TrainDataSet.getSharedItems(userID, Nuser).Count;
                double joined = TrainDataSet.getJoinedItems(userID, Nuser).Count;
                usersJWeights.Add(Nuser, (sheared == 0) ? 0 : joined / sheared);
            }

            return usersJWeights;
        }
    }
}
