using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniProject.Statistics;
using MiniProject.Data;

namespace hw2.Recommendation
{
    abstract class RecommendationBase<T, K, M, I> where I : IItem<T, K, M>
    {
        protected Predictor<T, K, M, I> Predictor;
        protected DataSet<T, K, M, I> TrainDataSet;

        public RecommendationBase(DataSet<T, K, M, I> train, Predictor<T, K, M, I> predictor)
        {
            this.TrainDataSet = train;
            this.Predictor = predictor;
        }

        public abstract List<M> Recommendation(T userID, int n);
    }
}
