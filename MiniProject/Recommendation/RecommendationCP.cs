using Assignment1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw2.Recommendation
{
    class RecommendationCP<T, K, M, I> : RecommendationBase<T, K, M, I> where I : IItem<T, K, M>
    {
        public RecommendationCP(DataSet<T, K, M, I> train) : base(train, null) {
            
        }

        /*
         * return Pr(item1 | item2)
         */ 
        private double PRItemItem(M item1, M item2)
        {     
            if (item1.Equals(item2))
            {
                return 0.0;
            }

            Dictionary<M, Dictionary<T, I>> userByItems = TrainDataSet.getUsersByItems();

            if (!userByItems.ContainsKey(item1) || !userByItems.ContainsKey(item2))
            {
                return 0.0;
            }

            Dictionary<T, I> userOfItem1 = userByItems[item1];
            Dictionary<T, I> userOfItem2 = userByItems[item2];

            int count = userOfItem1.Where(p => userOfItem2.ContainsKey(p.Key)).ToList().Count;

            if (count < 11)
            {
                return 0.0;
            }

            return ((double)count) / userOfItem2.Count;
        }

        /*
         * return Pr(item | user)
         */
        private double PRItemUser(M item, T user)
        {
            Dictionary<M, I> itemsOfUser = TrainDataSet.GetMatrixRating()[user];
            Dictionary<M, double> PR = new Dictionary<M, double>();

            foreach (M userItem in itemsOfUser.Keys)
            {
                if (PR.ContainsKey(userItem))
                {
                    PR[userItem] = Math.Max(PR[userItem], PRItemItem(item, userItem));
                }
                else
                {
                    PR.Add(userItem, PRItemItem(item, userItem));
                }
            }

            return PR.Values.Max();
        }

        public override List<M> Recommendation(T userID, int n)
        {
            Dictionary<M, I> itemsOfUser = TrainDataSet.GetMatrixRating()[userID];
            Dictionary<M, double> recommendedItems = new Dictionary<M, double>();

            foreach (M itemID in TrainDataSet.GetItems().GroupBy(x => x.GetShearedItemID()).Select(x => x.Key).ToList())
            {
                if (itemsOfUser.ContainsKey(itemID))
                {
                    continue;
                }

                if (!recommendedItems.ContainsKey(itemID))
                {
                    recommendedItems.Add(itemID, PRItemUser(itemID, userID));
                }
                else
                {
                    double pr = PRItemUser(itemID, userID);
                    recommendedItems[itemID] = Math.Max(recommendedItems[itemID], pr);
                }
            }

            List<M> topN = recommendedItems.OrderBy(p => -p.Value).Take(n).Select(p => p.Key).ToList();
            return topN;
        }
    }
}
