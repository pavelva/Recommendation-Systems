using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Statistics
{
    class PearsonDOFeatureVector<T, K, M, I> : Pearson<T, K, M, I> where I : IItem<T, K, M>
    {
        public PearsonDOFeatureVector(DataSet<T, K, M, I> dataSet) : base(dataSet) { }

        protected override double CalculateWeight(T activeUserID, T userID)
        {
            double FVMult = VMult(TrainDataSet.GetUserData(activeUserID.ToString()).getFeatureVector(), TrainDataSet.GetUserData(userID.ToString()).getFeatureVector());
            double weight = (FVMult / (TrainDataSet.GetUserData(activeUserID.ToString()).getFeatureVectorSize() * TrainDataSet.GetUserData(userID.ToString()).getFeatureVectorSize()));

            return weight;
        }

    }
}
