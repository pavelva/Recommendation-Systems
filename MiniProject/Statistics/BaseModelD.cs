using MiniProject.Data;
using MiniProject.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniProject.Statistics
{
    class BaseModelD<T, K, M, I> : BaseModel<T, K, M, I> where I : IItem<T, K, M>
    {
        public BaseModelD(DataSet<T, K, M, I> train, DataSet<T, K, M, I> validation, int vSize)
            : base(train, validation, vSize)
        {
            foreach(T UID in train.GetMatrixRating().Keys){
                addUserData(Pu[UID], train.GetUserData(UID.ToString()));
            }

            foreach (M IID in train.getUsersByItems().Keys)
            {
                addItemData(Qi[IID], train.getUsersByItems()[IID].Select(
                    p => train.GetUserData(p.Key.ToString()).getFeatureVector()).ToList());
            }
            VSize = Qi.First().Value.Count;
        }

        private void addItemData(List<double> qi, List<List<double>> usersFeatuers)
        {
            for (int i = 0; i < usersFeatuers[0].Count; i++)
            {
                double sum = 0.0;

                foreach (List<double> featuers in usersFeatuers)
                {
                    sum += featuers[i];
                }

                qi.Add((sum / usersFeatuers.Count) / 100);
            }
        }

        private void addUserData(List<double> pu, UserData userData)
        {
            for (int i = 0; i < userData.getFeatureVector().Count; i++)
            {
                pu.Add(userData.getFeatureVector()[i] / 100);
            }
        }
    }
}