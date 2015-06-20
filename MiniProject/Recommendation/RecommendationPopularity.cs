using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2.Recommendation
{
    class RecommendationPopularity<T, K, M, I> : RecommendationBase<T, K, M, I> where I : IItem<T, K, M>
    {
        public RecommendationPopularity(DataSet<T, K, M, I> train) : base(train, null) {}

        public override List<M> Recommendation(T userID, int n)
        {
            Dictionary<M, Dictionary<T, I>> usersByItems = TrainDataSet.getUsersByItems();
            List<M> recommendation = usersByItems.Keys.Where(m => !TrainDataSet.UserContainsItem(userID, m))
                .OrderBy(m => -usersByItems[m].Count).Take(n).ToList();

            return recommendation;
        }
    }
}
