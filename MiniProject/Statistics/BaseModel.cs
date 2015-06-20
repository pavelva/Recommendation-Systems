using MiniProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniProject.Statistics
{
    class BaseModel<T, K, M, I> : OfflinePredictor<T, K, M, I> where I : IItem<T, K, M>
    {
        protected Dictionary<T, double> Bu;
        protected Dictionary<M, double> Bi;
        protected double Mu;
        protected double Lambda;
        protected double Gamma;
        protected Dictionary<T, List<double>> Pu;
        protected Dictionary<M, List<double>> Qi;
        protected int VSize;

        public BaseModel(DataSet<T, K, M, I> train, DataSet<T, K, M, I> validation, int vSize) : base(train, validation)
        {
            Mu = TrainDataSet.getItemAverage();
            Lambda = 0.05;
            Gamma = 0.05;
            VSize = vSize;

            Qi = new Dictionary<M, List<double>>();
            Pu = new Dictionary<T, List<double>>();
            Bu = new Dictionary<T, double>();
            Bi = new Dictionary<M, double>();

            Random rand = new Random();

            foreach (I item in TrainDataSet.GetItems())
            {
                if(!Bu.ContainsKey(item.GetUserID()))
                {
                    Bu.Add(item.GetUserID(), rand.Next(-1, 1) / 100.0);
                    Pu.Add(item.GetUserID(), new List<double>());
                    int size = vSize;
                    while (size-- > 0)
                    {
                        Pu[item.GetUserID()].Add(rand.Next(-1, 1) / 100.0);
                    }
                }

                if (!Bi.ContainsKey(item.GetShearedItemID()))
                {
                    Bi.Add(item.GetShearedItemID(), rand.Next(-1, 1) / 100.0);
                    Qi.Add(item.GetShearedItemID(), new List<double>());
                    int size = vSize;
                    while (size-- > 0)
                    {
                        Qi[item.GetShearedItemID()].Add(rand.Next(-1, 1) / 100.0);
                    }
                }
            }
        }

        public override double PredictRating(T activeUserID, M shearedItemID)
        {
            double bu, bi;
            List<double> pu;
            List<double> qi;

            if (Bu.ContainsKey(activeUserID))
                bu = Bu[activeUserID];
            else
                bu = Bu.Values.Average();

            if (Bi.ContainsKey(shearedItemID))
                bi = Bi[shearedItemID];
            else
                bi = Bi.Values.Average();

            if (Pu.ContainsKey(activeUserID))
                pu = Pu[activeUserID];
            else
                pu = CalcAverage(Pu.Values, Pu.Count);

            if (Qi.ContainsKey(shearedItemID))
                qi = Qi[shearedItemID];
            else
                qi = CalcAverage(Qi.Values, Qi.Count);

            double p = Mu + bu + bi + VMult(pu, qi);
            return (p > 1) ? p : p;
        }

        private List<double> CalcAverage(IEnumerable<List<double>> vectors, int size)
        {
            List<double> average = new List<double>(new double[VSize]);

            foreach (List<double> vector in vectors)
            {
                for (int i = 0; i < vector.Count; i++)
                {
                    average[i] += vector[i];
                }
            }

            for (int i = 0; i < average.Count; i++)
            {
                average[i] /= size;
            }

            return average;
        }

        public override void TrainModel()
        {
            double prevRMSE = Double.MaxValue;
            Dictionary<T, double> tBu = CopyDic(Bu);
            Dictionary<M, double> tBi = CopyDic(Bi);
            Dictionary<T, List<double>> tPu = CopyDic(Pu);
            Dictionary<M, List<double>> tQi = CopyDic(Qi);

            while (true)
            {
                tBu = CopyDic(Bu);
                tBi = CopyDic(Bi);
                tPu = CopyDic(Pu);
                tQi = CopyDic(Qi);

                foreach (I item in TrainDataSet.GetItems())
                {
                    double PRui = PredictRating(item.GetUserID(), item.GetShearedItemID());
                    double Rui = TrainDataSet.GetRating(item.GetUserID(), item.GetShearedItemID());
                    double Eui = Rui - PRui;

                    Bu[item.GetUserID()] = nextParam(Eui, Bu[item.GetUserID()]);
                    Bi[item.GetShearedItemID()] = nextParam(Eui, Bi[item.GetShearedItemID()]);
                    nextParam(Eui, Pu[item.GetUserID()], Qi[item.GetShearedItemID()]);
                    nextParam(Eui, Qi[item.GetShearedItemID()], Pu[item.GetUserID()]);
                }

                double currentRMSE = RMSE(ValidationDataSet);
                if (currentRMSE > prevRMSE)
                {
                    Bu = CopyDic(tBu);
                    Bi = CopyDic(tBi);
                    Pu = CopyDic(tPu);
                    Qi = CopyDic(tQi);

                    return;
                }
                else
                {
                    prevRMSE = currentRMSE;

                }
            }
        }

        private Dictionary<R, double> CopyDic<R>(Dictionary<R, double> dic)
        {
            Dictionary<R, double> newDic = new Dictionary<R, double>();
            
            foreach(R item in dic.Keys)
            {
                newDic.Add(item, dic[item]);
            }

            return newDic;
        }

        private Dictionary<R, List<double>> CopyDic<R>(Dictionary<R, List<double>> dic)
        {
            Dictionary<R, List<double>> newDic = new Dictionary<R, List<double>>();
            
            foreach(R item in dic.Keys)
            {
                List<double> list = new List<double>(new double[dic[item].Count]);

                for(int i=0; i < dic[item].Count; i++)
                    list[i] = dic[item][i];
                
                newDic.Add(item, list);
            }

            return newDic;
        }

        private double nextParam(double Eui, double b)
        {
            return b + (Gamma * (Eui - (Lambda * b)));
        }

        private void nextParam(double Eui, List<double> list, List<double> list2)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = nextParam(Eui * list2[i], list[i]);
            }
        }

        public override double getUserDistances(T user1, T user2)
        {
            //if(UserDistances.ContainsKey(user1) && UserDistances[user1].ContainsKey(user2))
            //{
            //    return UserDistances[user1][user2];
            //}
            //if (UserDistances.ContainsKey(user2) && UserDistances[user2].ContainsKey(user1))
            //{
            //    return UserDistances[user2][user1];
            //}

            //if (!UserDistances.ContainsKey(user1))
            //{
            //    UserDistances.Add(user1, new Dictionary<T,double>());
            //}
            //if (!UserDistances.ContainsKey(user2))
            //{
            //    UserDistances.Add(user2, new Dictionary<T, double>());
            //}

            double weight = VDistance(Pu[user1], Pu[user2]);
            //UserDistances[user2].Add(user1, weight);
            //UserDistances[user1].Add(user2, weight);
            return weight;
        }
    }
}
